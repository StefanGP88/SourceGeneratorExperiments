using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public static class AttributeFinder
    {
        public static string FindClassByAttribute(this IncrementalGeneratorInitializationContext context, string fullAttributeName)
        {
            var classDeclarations = context.GetClassDeclarations();

            // todo find attribute: se enumgen.cs linie 146

            return "";
        }

        private static IncrementalValuesProvider<ClassDeclarationSyntax> GetClassDeclarations(this IncrementalGeneratorInitializationContext context)
        {
            return context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, cancelToken) =>
                {
                    return syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0; ;
                },
                transform: (ctx, cancelToken) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;
                    return classDeclarationSyntax;
                })
                .Where(provider => provider != null);
        }
    }
}
