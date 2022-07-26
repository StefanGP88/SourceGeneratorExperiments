using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenLib.Extensions
{
    public class FluentSyntaxClassFinder
    {
        private readonly IncrementalGeneratorInitializationContext _context;
        private readonly IncrementalValuesProvider<ClassDeclarationSyntax> _classDeclarations;
        private readonly IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> _compilatedClasses;


        public FluentSyntaxClassFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _classDeclarations = _context.SyntaxProvider.CreateSyntaxProvider
                (
                    predicate: (syntaxNode, cancelToken) => syntaxNode is ClassDeclarationSyntax,
                    transform: (ctx, cancelToken) => (ClassDeclarationSyntax)ctx.Node
                )
                .Where(provider => provider != null);

            _compilatedClasses = _context.CompilationProvider.Combine(_classDeclarations.Collect());
        }

        public void Build(Action<ClassFilter> bob)
        {
            _context.RegisterSourceOutput(_compilatedClasses, (sourceProductionContext, source) =>
            {
                var classFilter = new ClassFilter(source.Item1, source.Item2);
                bob(classFilter);

                if (classFilter._sourceBuilder == null)
                    return;

                if (!classFilter._foundClasses.Any())
                    return;

                var list = classFilter._foundClasses.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    var code = classFilter._sourceBuilder(list[i]);
                    var fileName = $"{classFilter.TemplateName}_{i}.g.cs";

                    sourceProductionContext.AddSource(fileName, code);
                }
            });
        }
    }
}