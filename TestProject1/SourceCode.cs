using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestProject1
{
    internal static class SourceCode
    {
        //todo expected result here
        internal static SyntaxTree WithAttribute => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    [TestClassMarker]
    public partial class WithAttributeTestClass
    {
        
    }
}
");
        internal static SyntaxTree WithoutAttribute => WithoutAnything;
        internal static SyntaxTree WithBaseClass => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    public partial class WithEverythingTestClass : TestBaseClass
    {
    }
}
");
        internal static SyntaxTree WithoutBaseClass => WithoutAnything;
        internal static SyntaxTree WithInterface => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.Unittest
{
    public partial class WithEverythingTestClass :  ITestInterFace
    {
    }
}
");
        internal static SyntaxTree WithoutInterface => WithoutAnything;
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
        internal static SyntaxTree WithoutMemberAttribute => WithoutAnything;
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
        internal static SyntaxTree WithoutAnything => CSharpSyntaxTree.ParseText(@"
namespace FluentSyntaxGenerator.UnitTest
{
    public partial class WithoutAnythingTestClass
    {

    }
}
");
    }
}
