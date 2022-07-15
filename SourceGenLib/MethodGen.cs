using Microsoft.CodeAnalysis;
using SourceGenLib.Extensions;
using SourceGenLib.Markers;

namespace SourceGenLib
{
    [Generator]
    public class MethodGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var AttributeFinder = new MethodAttributeFinder(context);
            AttributeFinder.BuildFromMethodAttribute<MyMethodMarkerAttribute>(x =>
            {
                return "some generated code";
            });
        }
    }
}
