using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestProject1
{
    internal static class SourceCode
    {
        internal static SyntaxTree WithAttributeExpectedResult => GetExpectedResult("WithAttributeTestClass");
        internal static SyntaxTree WithAttribute => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    [TestClassMarker]
    public partial class WithAttributeTestClass
    {
        
    }
}
");


        internal static SyntaxTree WithoutAttributeExpectedResult => GetExpectedResult("WithoutAnythingTestClass");
        internal static SyntaxTree WithoutAttribute => WithoutAnything;


        internal static SyntaxTree WithBaseClassExpectedResult => GetExpectedResult("WithBassClassTestClass");
        internal static SyntaxTree WithBaseClass => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    public partial class WithBassClassTestClass : TestBaseClass
    {
    }
}
");


        internal static SyntaxTree WithoutBaseClassExpectedResult => GetExpectedResult("WithoutAnythingTestClass");
        internal static SyntaxTree WithoutBaseClass => WithoutAnything;


        internal static SyntaxTree WithInterfaceExpectedResult => GetExpectedResult("WithInterfaceTestClass");
        internal static SyntaxTree WithInterface => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    public partial class WithInterfaceTestClass :  ITestInterFace
    {
    }
}
");


        internal static SyntaxTree WithoutInterfaceExpectedResult => GetExpectedResult("WithoutAnythingTestClass");
        internal static SyntaxTree WithoutInterface => WithoutAnything;


        internal static SyntaxTree WithMemberAttributeExpectedResult => GetExpectedResult("WithMemberAttributeTestClass");
        internal static SyntaxTree WithMemberAttribute => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    public partial class WithMemberAttributeTestClass
    {
        [TestMemberMarker]
        public void TestMemberMethod()
        {
        }
    }
}
");


        internal static SyntaxTree WithoutMemberAttributeExpectedResult => GetExpectedResult("WithoutAnythingTestClass");
        internal static SyntaxTree WithoutMemberAttribute => WithoutAnything;


        internal static SyntaxTree WithEverythingExpectedResult => GetExpectedResult("WithEverythingTestClass");
        internal static SyntaxTree WithEverything => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    [TestClassMarker]
    public partial class WithEverythingTestClass : TestBaseClass, ITestInterFace
    {
        [TestMemberMarker]
        public void TestMemberMethod()
        {
        }
    }
}
");


        internal static SyntaxTree WithoutAnythingExpectedResult => GetExpectedResult("WithoutAnythingTestClass");
        internal static SyntaxTree WithoutAnything => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.UnitTest
{
    public partial class WithoutAnythingTestClass
    {

    }
}
");

        internal static SyntaxTree GetExpectedResult(string className)
        {
            return CSharpSyntaxTree.ParseText(@$"
namespace FluentSyntaxGenerator.Unittest
{{
    public partial class {className}
    {{
        public void Print()
        {{
            System.Console.WriteLine(""Made with fluent generator"");
        }}
    }}
}}
");
        }
        internal static SyntaxTree Markers => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    [IgnoreInTest]
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class TestClassMarkerAttribute : System.Attribute
    {
    }

    [IgnoreInTest]
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class TestMemberMarkerAttribute : System.Attribute
    {
    }

    [IgnoreInTest]
    public interface ITestInterFace
    {
    }

    [IgnoreInTest]
    public class TestBaseClass
    {
    }

    [IgnoreInTest]
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public class IgnoreInTestAttribute : System.Attribute
    {
    }
}");
    }
}
