using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public class FoundPropertyContainer : FoundBaseMethodContainer<PropertyDeclarationSyntax>
    {
        public FoundPropertyContainer(PropertyDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
}