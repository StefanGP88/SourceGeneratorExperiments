using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenLib.Extensions
{
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
}