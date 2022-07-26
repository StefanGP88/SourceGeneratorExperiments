using Microsoft.CodeAnalysis;
using System;

namespace SourceGenLib.Extensions
{
    public static class FluentSyntaxExtensions
    {
        public static void CodeTemplate(this IncrementalGeneratorInitializationContext context, Action<ClassFilter> bob)
        {
            var biob = new FluentSyntaxClassFinder(context);
            biob.Build(bob);
        }
    }
}