using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenLib.Extensions
{
    public class FoundClassContainer
    {
        private readonly Compilation _compilation;

        public FoundClassContainer(ClassDeclarationSyntax declarationSyntax, INamedTypeSymbol semanticsModel, Compilation compilation)
        {
            DeclarationSyntax = declarationSyntax;
            Semantics = semanticsModel;
            _compilation = compilation;
        }
        public ClassDeclarationSyntax DeclarationSyntax { get; }
        public INamedTypeSymbol Semantics { get; }

        private string? _Namespace;
        public string Namespace
        {
            get
            {
                return _Namespace ??= Semantics.ContainingNamespace.ToString();
            }
        }

        private string? _Class;
        public string Class
        {
            get
            {
                return _Class ??= DeclarationSyntax.Identifier.ToString();
            }
        }

        private List<string>? _Modifiers;
        public List<string> Modifiers
        {
            get
            {
                return _Modifiers ??= DeclarationSyntax.Modifiers.Select(x => x.Text).ToList();
            }
        }
        public string AccessModifier
        {
            get
            {
                return Modifiers[0];
            }
        }

        private List<FoundAttributeContainer>? _FoundAttributes;
        public List<FoundAttributeContainer> FoundAttributes
        {
            get
            {
                return _FoundAttributes ??= Semantics.GetAttributes().Select(x => new FoundAttributeContainer(x)).ToList();
            }
        }

        private List<FoundMethodContainer>? _Methods;
        public List<FoundMethodContainer> Methods
        {
            get
            {
                return _Methods ??= DeclarationSyntax.Members
                    .Where(x => x is MethodDeclarationSyntax && _compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) is IMethodSymbol)
                    .Select(x => new FoundMethodContainer((MethodDeclarationSyntax)x, (IMethodSymbol)_compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!))
                    .ToList();
            }
        }

        private List<FoundConstructorContainer>? _Constructors;
        public List<FoundConstructorContainer> Constructors
        {
            get
            {
                return _Constructors ??= DeclarationSyntax.Members
                    .Where(x => x is ConstructorDeclarationSyntax && _compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) is IMethodSymbol)
                    .Select(x => new FoundConstructorContainer((ConstructorDeclarationSyntax)x, (IMethodSymbol)_compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!))
                    .ToList();
            }
        }

        private List<FoundDestructorContainer>? _Destructor;
        public List<FoundDestructorContainer> Destructor
        {
            get
            {
                return _Destructor ??= DeclarationSyntax.Members
                    .Where(x => x is DestructorDeclarationSyntax && _compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) is IMethodSymbol)
                    .Select(x => new FoundDestructorContainer((DestructorDeclarationSyntax)x, (IMethodSymbol)_compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!))
                    .ToList();
            }
        }

        private List<FoundPropertyContainer>? _Properties;
        public List<FoundPropertyContainer> Properties
        {
            get
            {
                return _Properties ??= DeclarationSyntax.Members
                    .Where(x => x is PropertyDeclarationSyntax && _compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) is IMethodSymbol)
                    .Select(x => new FoundPropertyContainer((PropertyDeclarationSyntax)x, (IMethodSymbol)_compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!))
                    .ToList();
            }
        }

        private List<FoundFieldContainer>? _Fields;
        public List<FoundFieldContainer> Fields
        {
            get
            {
                return _Fields ??= DeclarationSyntax.Members
                    .Where(x => x is PropertyDeclarationSyntax && _compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) is IMethodSymbol)
                    .Select(x => new FoundFieldContainer((FieldDeclarationSyntax)x, (IMethodSymbol)_compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!))
                    .ToList();
            }
        }
    }
}