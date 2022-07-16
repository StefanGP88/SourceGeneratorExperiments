using Microsoft.CodeAnalysis;
using SourceGenLib.Extensions;
using SourceGenLib.Markers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGenLib
{
    [Generator]
    public class ClassGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var AttributeFinder = new ClassAttributeFinder(context);
            AttributeFinder.BuildFromClassAttribute<MyClassMarkerAttribute>(x =>
            {
                 return "some generated code";
            });
        }
    }

    [Generator]
    public class BaseClassGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x=> x.SetCodeTemplate(x=> ""));

            var AttributeFinder = new BaseClassFinder(context);
            AttributeFinder.BuildFromClassBaseClass<TestBaseClass>(x =>
            {
                return "some generated code";
            });
        }
    }
}
