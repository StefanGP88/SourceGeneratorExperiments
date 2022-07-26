using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenLib.Extensions
{
    public class FluentSyntaxClassFinder
    {
        private IncrementalGeneratorInitializationContext _context;
        private IncrementalValuesProvider<ClassDeclarationSyntax> _classDeclarations;
        private IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> _compilatedClasses;


        public FluentSyntaxClassFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _classDeclarations = _context.SyntaxProvider.CreateSyntaxProvider
                (
                    predicate: (syntaxNode, cancelToken) => syntaxNode is ClassDeclarationSyntax,
                    transform: (ctx, cancelToken) => (ClassDeclarationSyntax)ctx.Node
                )
                .Where(provider => provider != null);

            _compilatedClasses = _context.CompilationProvider.Combine(_classDeclarations.Collect());
        }

        public void Build(Action<ClassFilter> bob)
        {
            _context.RegisterSourceOutput(_compilatedClasses, (sourceProductionContext, source) =>
            {
                var classFilter = new ClassFilter(source.Item1, source.Item2);
                bob(classFilter);

                if (classFilter._sourceBuilder == null)
                    return;

                if (!classFilter._foundClasses.Any())
                    return;

                var list = classFilter._foundClasses.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    var code = classFilter._sourceBuilder(list[i]);
                    var fileName = $"{classFilter.TemplateName}_{i}.g.cs";

                    sourceProductionContext.AddSource(fileName, code);
                }
            });
        }
    }

    public static class FluentSyntaxExtensions
    {
        public static void CodeTemplate(this IncrementalGeneratorInitializationContext context, Action<ClassFilter> bob)
        {
            var biob = new FluentSyntaxClassFinder(context);
            biob.Build(bob);
        }
    }

    public class ClassFilter
    {
        public Compilation _compilation;
        public IEnumerable<FoundClassContainer> _foundClasses;
        public Func<FoundClassContainer, string>? _sourceBuilder;
        public string? TemplateName;
        public ClassFilter(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes)
        {
            _compilation = compilation;
            var foundClassList = new List<FoundClassContainer>();

            foreach (var cds in classes)
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(cds.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(cds) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }
                foundClassList.Add(new FoundClassContainer(cds, classSymbol, _compilation));
            }
            _foundClasses = foundClassList;
        }

        public ClassFilter WithAttribute<T>()
        {
            if (!Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.GetAttributes()
                    .Any(z =>Equals(attrib!.ToString(), z.ToString()));
            }).ToList();
            return this;
        }
        public ClassFilter WithoutAttribute<T>()
        {
            if (!Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.GetAttributes()
                    .All(z => !Equals(attrib!.ToString(), z.ToString()));
            }).ToList();
            return this;
        }
        public ClassFilter WithBaseClass<T>()
        {
            if (!Exists<T>(out var baseClass))
            {
                return this;
            }

            _foundClasses = _foundClasses
                    .Where(x => Equals(baseClass!.ToString(), x.Semantics.BaseType?.ToString()))
                    .ToList();

            return this;
        }
        public ClassFilter WithoutBaseClass<T>()
        {
            if (!Exists<T>(out var baseClass))
            {
                return this;
            }

            _foundClasses = _foundClasses
                    .Where(x => !Equals(baseClass!.ToString(), x.Semantics.BaseType?.ToString()))
                    .ToList();

            return this;
        }
        public ClassFilter WithInterface<T>()
        {
            if (!Exists<T>(out var interFace))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.Interfaces
                    .Any(z => Equals(interFace!.ToString(), z.ToString()));
            }).ToList();

            return this;
        }
        public ClassFilter WithoutInterface<T>()
        {
            if (!Exists<T>(out var interFace))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.Interfaces
                    .All(z => !Equals(interFace!.ToString(), z.ToString()));
            }).ToList();

            return this;
        }
        public ClassFilter WithMemberAttribute<T>()
        {
            if (!Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.GetMembers().Any(z =>
                {
                    return z.GetAttributes()
                        .Any(y => Equals(attrib!.ToString(), y.ToString()));
                });
            }).ToList();

            return this;
        }
        public ClassFilter WithoutMemberAttribute<T>()
        {
            if (!Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.GetMembers().All(z =>
                {
                    return z.GetAttributes()
                        .All(y => !Equals(attrib!.ToString(), y.ToString()));
                });
            }).ToList();

            return this;
        }


        public ClassFilter SetCodeTemplate(string templateName, Func<FoundClassContainer, string>? sourceBuilder)
        {
            TemplateName = templateName;
            _sourceBuilder = sourceBuilder;
            return this;
        }
        private bool Exists<T>(out INamedTypeSymbol? symbol)
        {
            symbol = _compilation.GetTypeByMetadataName(typeof(T).FullName);
            return symbol != null;
        }
    }

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

    public class FoundFieldContainer : FoundBaseMethodContainer<FieldDeclarationSyntax>
    {
        public FoundFieldContainer(FieldDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
    public class FoundPropertyContainer : FoundBaseMethodContainer<PropertyDeclarationSyntax>
    {
        public FoundPropertyContainer(PropertyDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }
    public class FoundDestructorContainer : FoundBaseMethodContainer<DestructorDeclarationSyntax>
    {
        public FoundDestructorContainer(DestructorDeclarationSyntax destructorDeclarationSyntax, IMethodSymbol semntics)
            : base(destructorDeclarationSyntax, semntics)
        { }
    }
    public class FoundConstructorContainer : FoundBaseMethodContainer<ConstructorDeclarationSyntax>
    {
        public FoundConstructorContainer(ConstructorDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }

    }
    public class FoundMethodContainer : FoundBaseMethodContainer<MethodDeclarationSyntax>
    {
        public FoundMethodContainer(MethodDeclarationSyntax methodDeclarationSyntax, IMethodSymbol semantics)
            : base(methodDeclarationSyntax, semantics)
        { }
    }

    public class FoundBaseMethodContainer<T> where T : MemberDeclarationSyntax
    {
        public FoundBaseMethodContainer(T methodDeclarationSyntax, IMethodSymbol semantics)
        {
            DeclarationSyntax = methodDeclarationSyntax;
            Semantics = semantics;
        }
        public IMethodSymbol Semantics { get; }
        public T DeclarationSyntax { get; }

        private string? _Method;
        public string Method
        {
            get
            {
                return _Method ??= Semantics.ToString();
            }
        }

        private List<string>? _Modifiers;
        public List<string> Modifiers
        {
            get
            {
                return _Modifiers ??= DeclarationSyntax.Modifiers
                    .Select(x => x.Text)
                    .ToList();
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
                return _FoundAttributes ??= Semantics.GetAttributes()
                    .Select(x => new FoundAttributeContainer(x))
                    .ToList();
            }
        }

        //TODO: would probably make sense to have args in a "FoundContainer" class to access attributes 
        private Dictionary<string, int>? _ArgIndexes;
        public Dictionary<string, int> ArgIndexes
        {
            get
            {
                return _ArgIndexes ??= Semantics.Parameters
                    .Select((x, i) => (name: x.Name, index: i))
                    .ToDictionary(x => x.name, x => x.index);
            }
        }

        private Dictionary<int, string>? _ArgTypes;
        public Dictionary<int, string> ArgTypes
        {
            get
            {
                return _ArgTypes ??= Semantics.Parameters
                    .Select((x, i) => (type: x.Type.Name, index: i))
                    .ToDictionary(x => x.index, x => x.type);
            }
        }
    }


    public class FoundAttributeContainer
    {
        public FoundAttributeContainer(AttributeData attributeData)
        {
            AttributeData = attributeData;
        }

        public AttributeData AttributeData { get; set; }

        private string? _Attribute;
        public string Attribute
        {
            get
            {
                return _Attribute ??= AttributeData.AttributeClass!.ToString();
            }
        }

        private List<object?>? _CtorArgs;
        public List<object?> ConstructorArgs
        {
            get
            {
                return _CtorArgs ??= AttributeData.ConstructorArguments
                    .Select(x => x.Value)
                    .ToList();
            }
        }

        private Dictionary<string, object?>? _NamedArgs;
        public Dictionary<string, object?> NamedArgs
        {
            get
            {
                return _NamedArgs ??= AttributeData.NamedArguments
                    .ToDictionary(x => x.Key, x => x.Value.Value);
            }
        }
    }
}