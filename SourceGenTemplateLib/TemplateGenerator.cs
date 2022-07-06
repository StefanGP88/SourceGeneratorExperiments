using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace SourceGenTemplateLib
{
    [Generator]
    public class TemplateGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            //context.RegisterForPostInitialization(cb =>
            //{
            //    var currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            //    var fullPath = Path.Combine(currentFolder, "Templates");
            //    var files = Directory.GetFiles(fullPath, "*.cs");

            //    foreach (var item in files)
            //    {
            //        if (!File.Exists(item))
            //            continue;

            //        var txt = File.ReadAllText(item);
            //        var fileName = Path.GetFileNameWithoutExtension(item);

            //        string methodTemplate = TemplateHelperClass
            //            .Replace("_TemplateName", fileName)
            //            .Replace("_TemplateCode", txt);

            //        cb.AddSource(fileName + ".g.cs", SourceText.From(methodTemplate, Encoding.UTF8));

            //        //context.RegisterPostInitializationOutput(ctx => ctx.AddSource(fileName + ".g.cs", methodTemplate));
            //    }
            //});
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("myGeneratedFile.cs", SourceText.From(@"
namespace GeneratedNamespace
{
    public partial class GeneratedClass
    {
        public static void GeneratedMethod3()
        {
            // generated code
        }
    }
}", Encoding.UTF8));

            context.AddSource("myGeneratedFile1.cs", SourceText.From(@"
namespace GeneratedNamespace
{
    public partial class GeneratedClass
    {
        public static void GeneratedMethod1()
        {
            // generated code
        }
    }
}", Encoding.UTF8));


            context.AddSource("myGeneratedFile2.cs", SourceText.From(@"
namespace GeneratedNamespace
{
    public partial class GeneratedClass
    {
        public static void GeneratedMethod2()
        {
            // generated code
        }
    }
}", Encoding.UTF8));


            context.AddSource("myGeneratedFile4.cs", SourceText.From(@"
namespace GeneratedNamespace
{
    public partial class GeneratedClass
    {
        public static void GeneratedMethod4()
        {
            // generated code
        }
    }
}", Encoding.UTF8));



            var currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(currentFolder, "Templates");
            var files = Directory.GetFiles(fullPath, "*.cs");

            foreach (var item in files)
            {
                if (!File.Exists(item))
                    continue;

                var txt = File.ReadAllText(item);
                txt=  txt.Replace(Environment.NewLine, " ").Replace("  ", " ");
                var fileName = Path.GetFileNameWithoutExtension(item);

                string methodTemplate = TemplateHelperClass
                    .Replace("_TemplateName", fileName)
                    .Replace("_TemplateCode", txt);

                context.AddSource(fileName + ".cs", SourceText.From(methodTemplate, Encoding.UTF8));

            }
        }

        string TemplateHelperClass = @"namespace GeneratedNamespace
{
    public partial class GeneratedClass
    {
        public static string _TemplateName()
        {
            return @""_TemplateCode"";
        }
    }
}";

    }
}