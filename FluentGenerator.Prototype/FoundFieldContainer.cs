using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public class FoundFieldContainer : FoundBaseMethodContainer<FieldDeclarationSyntax>
    {
        public FoundFieldContainer(FieldDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
}