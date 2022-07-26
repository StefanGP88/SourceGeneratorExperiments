using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenLib.Extensions
{
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