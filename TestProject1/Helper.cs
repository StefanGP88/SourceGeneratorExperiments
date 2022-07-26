using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace FluentSyntaxGenerator.Unittest
{
    internal static class Helper
    {
        internal static string TestMarkers = @"
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
}";

        internal static void AddMarkers(this IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource
            (
                "TestClassMarker.g.cs",
                SourceText.From(TestMarkers, Encoding.UTF8))
            );
        }

        internal static string CodeToGenerate(string nameSpace, string className)
        {
            return @$"
                namespace {nameSpace}
                {{
                    public partial class {className}
                    {{
                        public void Print()
                        {{
                            System.Console.WriteLine(""Made with fluent generator"");
                        }}
                    }}
                }}";
        }
    }
}
