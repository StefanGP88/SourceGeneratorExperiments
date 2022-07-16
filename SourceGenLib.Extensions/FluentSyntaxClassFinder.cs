using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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
        Func<MyClassInfo, string>? _sourceBuilder;
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
                foundClassList.Add(new FoundClassContainer(cds, classSymbol));
            }
            _foundClasses = foundClassList;
        }

        public ClassFilter WithAttribute<T>()
        {
            if (Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.GetAttributes().Any(z => 
                {
                    if(z.AttributeClass != null)
                        return z.AttributeClass.Equals(attrib, SymbolEqualityComparer.Default);
                    return false;
                });

            });
            return this;
        }

        public ClassFilter WithBaseClass<T>()
        {
            if (Exists<T>(out var baseClass))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                if(x.Semantics.BaseType != null)
                    return x.Semantics.BaseType.Equals(baseClass, SymbolEqualityComparer.Default);
                return false;

            });

            return this;
        }

        public ClassFilter WithInterface<T>()
        {
            if (Exists<T>(out var interFace))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return x.Semantics.Interfaces.Any(x=>x.Equals(interFace, SymbolEqualityComparer.Default));
            });

            return this;
        }

        public ClassFilter WithMemberAttribute<T>()
        {
            if (Exists<T>(out var attrib))
            {
                return this;
            }

            _foundClasses = _foundClasses.Where(x =>
            {
                return  x.Semantics.GetMembers().Any(x=>x.Equals(attrib, SymbolEqualityComparer.Default));
            });

            return this;
        }

        public ClassFilter SetCodeTemplate(Func<MyClassInfo, string>? sourceBuilder)
        {
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
        public FoundClassContainer(ClassDeclarationSyntax declarationSyntax, INamedTypeSymbol semanticsModel)
        {
            DeclarationSyntax = declarationSyntax;
            Semantics = semanticsModel;
        }
        public ClassDeclarationSyntax DeclarationSyntax { get; set; }
        public INamedTypeSymbol Semantics { get; set; }
    }
}
