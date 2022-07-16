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
    public class BaseClassFinder
    {
        private IncrementalGeneratorInitializationContext _context;
        private IncrementalValuesProvider<ClassDeclarationSyntax> _classDeclarations;
        private IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> _compilatedClasses;

        public BaseClassFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _classDeclarations = _context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, cancelToken) =>
                {
                    return syntaxNode is ClassDeclarationSyntax cds && cds.BaseList != null;
                },
                transform: (ctx, cancelToken) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;
                    return classDeclarationSyntax;
                })
                .Where(provider => provider != null);

            _compilatedClasses = _context.CompilationProvider.Combine(_classDeclarations.Collect());


            var asdfg = _context.CompilationProvider;
        }
        public void BuildFromClassBaseClass<T>(Func<MyClassInfo, string> sourceBuilder)
        {
            _context.RegisterSourceOutput(_compilatedClasses, (sourceProductionContext, source) =>
            {
                var compilation = source.Item1;
                var classes = source.Item2;

                if (classes.IsDefaultOrEmpty) return;

                IEnumerable<ClassDeclarationSyntax> classDeclarations = classes.Distinct();

                var classToGenerate = GetTypesToGenerate<T>(compilation, classDeclarations, sourceProductionContext.CancellationToken);

                foreach (var myClassInfo in classToGenerate)
                {
                    var generatedCode = sourceBuilder(myClassInfo);
                    var generatedFileName = myClassInfo.ClassName + ".g.cs";

                    sourceProductionContext.AddSource(generatedFileName, SourceText.From(generatedCode, Encoding.UTF8));
                }
            });
        }
        private List<MyClassInfo> GetTypesToGenerate<T>(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, CancellationToken cancellationToken)
        {
            var result = new List<MyClassInfo>();
            var type = typeof(T);

            foreach (ClassDeclarationSyntax classDeclarationSyntax in classes)
            {
                var myClassInfo = new MyClassInfo();

                cancellationToken.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }
                if(classSymbol.BaseType?.ToString() != type.FullName)
                {
                    continue;
                }

                myClassInfo.ClassName = classSymbol.ToString();
                myClassInfo.NameSpace = classSymbol.ContainingNamespace.ToString();
                myClassInfo.Modifiers = classDeclarationSyntax.Modifiers.Select(x => x.Text).ToList();
                myClassInfo.InherritsFrom = classSymbol.BaseType?.ToString();
                myClassInfo.Interfaces = classSymbol.Interfaces.Select(x => x.Name).ToList();

                result.Add(myClassInfo);
            }

            return result;
        }
    }
}
