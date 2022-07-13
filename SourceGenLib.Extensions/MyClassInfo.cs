using System.Collections.Generic;
using System.Globalization;

namespace SourceGenLib.Extensions
{
    public class MyClassInfo
    {
        public MyAttributeInfo? Attribute { get; set; }
        public string? ClassName { get; set; }
        public string? NameSpace { get; set; }
        public List<string> Modifiers { get; set; } = new List<string> { };
        public string AccessModifier { get { return Modifiers[0]!; } }
        public string? InherritsFrom { get; set; }
        public List<string>? Interfaces { get; set; }

    }
    public class MyAttributeInfo
    {
        public List<object?> CtorArgs { get; set; } = new List<object?>();
        public Dictionary<string, object?> NamedArgs { get; set; } = new Dictionary<string, object?>();
    }

    public class MyMethodInfo
    {
        public MyAttributeInfo? Attribute { get; set; }
        public MyClassInfo? ContainingClass { get; set; }
        public string? MethodName { get; set; }
        public List<string> Modifiers { get; set; } = new List<string> { };
        public string AccessModifier { get { return Modifiers[0]!; } }
        public Dictionary<string, int> ArgsIndexes { get; set; } = new Dictionary<string, int>();
        public Dictionary<int, string> ArgsTypes { get; set; } = new Dictionary<int, string>();

    }
}
