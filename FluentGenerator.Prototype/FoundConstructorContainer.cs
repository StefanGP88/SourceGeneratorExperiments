using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public class FoundConstructorContainer : FoundBaseMethodContainer<ConstructorDeclarationSyntax>
    {
        public FoundConstructorContainer(ConstructorDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
}