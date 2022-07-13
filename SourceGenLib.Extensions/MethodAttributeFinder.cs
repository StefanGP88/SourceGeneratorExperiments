using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using System.Linq;

namespace SourceGenLib.Extensions
{
    public class MethodAttributeFinder
    {
        private IncrementalGeneratorInitializationContext _context;
        private IncrementalValuesProvider<MethodDeclarationSyntax> _methodDeclarations;
        private IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)> _compilatedMethods;

        public MethodAttributeFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _methodDeclarations = _context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, cancelToken) =>
                {
                    return syntaxNode is MethodDeclarationSyntax mds && mds.AttributeLists.Count > 0;
                },
                transform: (ctx, cancelToken) =>
                {
                    var methodDeclarationSyntax = (MethodDeclarationSyntax)ctx.Node;
                    return methodDeclarationSyntax;
                })
                .Where(provider => provider != null);

            _compilatedMethods = _context.CompilationProvider.Combine(_methodDeclarations.Collect());

        }
        public void BuildFromMethodAttribute<T>(Func<MyMethodInfo, string> sourceBuilder)
        {
            _context.RegisterSourceOutput(_compilatedMethods, (sourceProductionContext, source) =>
            {
                var compilation = source.Item1;
                var methods = source.Item2;

                if (methods.IsDefaultOrEmpty) return;

                IEnumerable<MethodDeclarationSyntax> methodDeclarations = methods.Distinct();

                var methodsToGenerate = GetTypesToGenerate<T>(compilation, methodDeclarations, sourceProductionContext.CancellationToken);

                foreach (var myMethodInfo in methodsToGenerate)
                {
                    var generatedCode = sourceBuilder(myMethodInfo);
                    var generatedFileName = myMethodInfo.MethodName + ".g.cs";

                    sourceProductionContext.AddSource(generatedFileName, SourceText.From(generatedCode, Encoding.UTF8));
                }
            });
        }
        private List<MyMethodInfo> GetTypesToGenerate<T>(Compilation compilation, IEnumerable<MethodDeclarationSyntax> methods, CancellationToken cancellationToken)
        {
            var result = new List<MyMethodInfo>();
            var type = typeof(T);

            INamedTypeSymbol? methodAttribute = compilation.GetTypeByMetadataName(type.FullName);
            INamedTypeSymbol? asd = compilation.GetTypeByMetadataName(type.Name);

            if (methodAttribute == null) return result;

            foreach (MethodDeclarationSyntax methodDeclarationSyntax in methods)
            {
                var myMethodInfo = new MyMethodInfo();

                cancellationToken.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(methodDeclarationSyntax) is not IMethodSymbol methodSymbol)
                {
                    continue;
                }

                myMethodInfo.MethodName = methodSymbol.Name;
                myMethodInfo.Modifiers = methodDeclarationSyntax.Modifiers.Select(x => x.Text).ToList();

                if (!methodSymbol.Parameters.IsEmpty)
                {
                    for (int i = 0; i < methodSymbol.Parameters.Length; i++)
                    {
                        myMethodInfo.ArgsIndexes[methodSymbol.Parameters[i].Name] = i;
                        myMethodInfo.ArgsTypes[i] = methodSymbol.Parameters[i].Type.Name;
                    }
                }

                foreach (AttributeData attributeData in methodSymbol.GetAttributes())
                {
                    if (!methodAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        continue;
                    }

                    if (attributeData.ConstructorArguments.IsEmpty && attributeData.NamedArguments.IsEmpty)
                    {
                        continue;
                    }

                    myMethodInfo.Attribute = new MyAttributeInfo();

                    if (!attributeData.ConstructorArguments.IsEmpty)
                    {
                        myMethodInfo.Attribute.CtorArgs = attributeData.ConstructorArguments
                            .Select(x => x.Value)
                            .ToList();
                    }

                    if (!attributeData.NamedArguments.IsEmpty)
                    {
                        myMethodInfo.Attribute.NamedArgs = attributeData.NamedArguments
                            .ToDictionary(x => x.Key, x => x.Value.Value);
                    }
                }

                result.Add(myMethodInfo);
            }

            return result;
        }
    }
}
