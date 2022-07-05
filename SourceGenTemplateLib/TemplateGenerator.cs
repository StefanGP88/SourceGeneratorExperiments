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
    public class TemplateGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(currentFolder, "Templates");
            var files = Directory.GetFiles(fullPath, "*.cs");

            foreach (var item in files)
            {
                if (!File.Exists(item))
                    continue;

                var txt = File.ReadAllText(item);
                var fileName = Path.GetFileNameWithoutExtension(item);

                string methodTemplate = TemplateHelperClass
                    .Replace("_TemplateName", fileName)
                    .Replace("_TemplateCode", txt);



                //context.RegisterPostInitializationOutput(ctx => ctx.AddSource(fileName + ".g.cs", methodTemplate));
            }
        }


        string TemplateHelperClass = @"namespace SourceGenTemplateLib
{
    public static partial class TemplateHelper
    {
        public static partial string _TemplateName()
        {
            return ""_TemplateCode"";
        }
    }
}";

    }
}

