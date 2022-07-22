using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SourceGenLib
{
    internal static class Helper
    {
        internal static string TestMarkers = @"
namespace FluentSyntaxGenerator.Unittest
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class TestClassMarkerAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class TestMemberMarkerAttribute : System.Attribute
    {
    }

    public interface ITestInterFace
    {
    }

    public class TestBaseClass
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
