using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceGenLib;
using SourceGenTemplateLib;
using System.Linq;
using System.Reflection;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EnumGen()
        {
            var inputCompilation = CreateCompilation(@"
using SourceGenLib;
namespace MyCode
{
    public class Program
    {

        //static void Main(string[] args)
        //{
        //    var c = Colour.Red;
        //    c.ToStringFast();
        //}

        [EnumExtensions(""TestExtensionEnumClass"")]
        public enum Colour
        {
            Red = 0,
            Green = 1,
            Blue = 2,
        }
    }
}
");
            var generator = new EnumGen();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            // We can now assert things about the resulting compilation:
            var empty = diagnostics.IsEmpty; // there were no diagnostics created by the generators
            var count = outputCompilation.SyntaxTrees.Count(); // we have two syntax trees, the original 'user' provided one, and the one added by the generator
            var giag = outputCompilation.GetDiagnostics(); // verify the compilation with the added source has no diagnostics


            var runResult = driver.GetRunResult();

            // The runResult contains the combined results of all generators passed to the driver
            var length = runResult.GeneratedTrees.Length;
            var empty2 = runResult.Diagnostics.IsEmpty;

            // Or you can access the individual results on a by-generator basis

        }


        private static Compilation CreateCompilation(string source)
        {
            return CSharpCompilation.Create("compilation",
                 new[] { CSharpSyntaxTree.ParseText(source) },
                 new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                 new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }
    }
}

namespace SourceGenTemplateLib
{
    public partial class TemplateHelper
    {
        public static string BobTemplate()
        {
            return @"namespace SourceGenTemplateLib
{
    public static class BobTemplate
    {
        public static void  Bob()
        {
            var x = 2M;
            System.Console.WriteLine(x);
        }
    }
}
";
        }
    }
}