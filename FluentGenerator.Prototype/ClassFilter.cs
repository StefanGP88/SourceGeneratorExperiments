using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenLib.Extensions
{
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
                    .Any(z => Equals(attrib!.ToString(), z.ToString()));
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
}