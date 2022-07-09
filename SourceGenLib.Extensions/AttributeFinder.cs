using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SourceGenLib.Extensions
{
    public class AttributeFinder
    {
        private IncrementalGeneratorInitializationContext _context;
        private IncrementalValuesProvider<ClassDeclarationSyntax> _classDeclarations;
        private IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> _compilatedClasses;

        public AttributeFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _classDeclarations = _context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, cancelToken) =>
                {
                    return syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0;
                },
                transform: (ctx, cancelToken) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;
                    return classDeclarationSyntax;
                })
                .Where(provider => provider != null);

            _compilatedClasses = _context.CompilationProvider.Combine(_classDeclarations.Collect());

        }

        public void FindClassByAttribute<T>(Func<MyClassInfo, string> sourceBuilder)
        {
            var t = typeof(T);
            _context.RegisterSourceOutput(_compilatedClasses, (sourceProductionContext, source) =>
            {
                var compilation = source.Item1;
                var classes = source.Item2;

                if (classes.IsDefaultOrEmpty) return;

                IEnumerable<ClassDeclarationSyntax> classDeclarations = classes.Distinct();

                var classToGenerate = GetTypesToGenerate(compilation, classDeclarations, sourceProductionContext.CancellationToken);

                foreach (var myClassInfo in classToGenerate)
                {
                    var generatedCode = sourceBuilder(myClassInfo);
                    var generatedFileName = myClassInfo.ClassName + ".g.cs";

                    sourceProductionContext.AddSource(generatedFileName, SourceText.From(generatedCode, Encoding.UTF8));
                }
            });
        }


        private static List<MyClassInfo> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> enums, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


    }
}
