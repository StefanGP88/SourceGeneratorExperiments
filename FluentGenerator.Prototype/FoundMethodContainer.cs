using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public class FoundMethodContainer : FoundBaseMethodContainer<MethodDeclarationSyntax>
    {
        public FoundMethodContainer(MethodDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
}