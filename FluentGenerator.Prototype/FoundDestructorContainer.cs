using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenLib.Extensions
{
    public class FoundDestructorContainer : FoundBaseMethodContainer<DestructorDeclarationSyntax>
    {
        public FoundDestructorContainer(DestructorDeclarationSyntax destructorDeclarationSyntax, IMethodSymbol semntics)
            : base(destructorDeclarationSyntax, semntics)
        { }
    }
}