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

        public void BuildFromClassAttribute<T>(Func<MyClassInfo, string> sourceBuilder)
        {
            var t = typeof(T);
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

            INamedTypeSymbol? classAttribute = compilation.GetTypeByMetadataName(type.FullName);
            INamedTypeSymbol? asd = compilation.GetTypeByMetadataName(type.Name);

            if (classAttribute == null) return result;

            foreach (ClassDeclarationSyntax classDeclarationSyntax in classes)
            {
                var myClassInfo = new MyClassInfo();

                cancellationToken.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }
                else
                {
                    myClassInfo.ClassName = classSymbol.ToString();
                    myClassInfo.NameSpace = classSymbol.ContainingNamespace.ToString();
                    myClassInfo.Modifiers = classDeclarationSyntax.Modifiers.Select(x => x.Text).ToList();
                    myClassInfo.InherritsFrom = classSymbol.BaseType?.ToString();
                    myClassInfo.Interfaces = classSymbol.Interfaces.Select(x=>x.Name).ToList();
                    var access = myClassInfo.AccessModifier;
                }

                foreach (AttributeData attributeData in classSymbol.GetAttributes())
                {
                    if (!classAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        continue;
                    }

                    if(attributeData.ConstructorArguments.IsEmpty && attributeData.NamedArguments.IsEmpty)
                    {
                        continue;
                    }

                    myClassInfo.Attribute = new MyAttributeInfo();

                    if (!attributeData.ConstructorArguments.IsEmpty)
                    {
                        myClassInfo.Attribute.CtorArgs = attributeData.ConstructorArguments
                            .Select(x => x.Value)
                            .ToList();
                    }

                    if (!attributeData.NamedArguments.IsEmpty)
                    {
                        myClassInfo.Attribute.NamedArgs = attributeData.NamedArguments
                            .ToDictionary(x => x.Key, x => x.Value.Value);
                    }
                }
            }

            return result;
        }


    }
}
