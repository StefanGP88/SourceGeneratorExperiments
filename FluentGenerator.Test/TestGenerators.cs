using Microsoft.CodeAnalysis;
using SourceGenLib.Extensions;

namespace FluentSyntaxGenerator.Unittest
{
    [Generator]
    public class WithAttributeTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithAttributeTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithAttribute<TestClassMarkerAttribute>();
            });
        }
    }

    [Generator]
    public class WithoutAttributeTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithoutAttributeTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithoutAttribute<IgnoreInTestAttribute>()
                .WithoutAttribute<TestClassMarkerAttribute>();
            });
        }
    }

    [Generator]
    public class WithBaseClassTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithBaseClassTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithBaseClass<TestBaseClass>();
            });
        }
    }

    [Generator]
    public class WithoutBaseClassTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithoutBaseClassTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithoutAttribute<IgnoreInTestAttribute>()
                .WithoutBaseClass<TestBaseClass>();
            });
        }
    }

    [Generator]
    public class WithInterfaceTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithInterfaceTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithInterface<ITestInterFace>();
            });
        }
    }

    [Generator]
    public class WithoutInterfaceTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithoutInterfaceTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithoutAttribute<IgnoreInTestAttribute>()
                .WithoutInterface<ITestInterFace>();
            });
        }
    }

    [Generator]
    public class WithMemberAttributeTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithMemberAttributeTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithMemberAttribute<TestMemberMarkerAttribute>();
            });
        }
    }

    [Generator]
    public class WithoutMemberAttributeTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithoutMemberAttributeTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithoutAttribute<IgnoreInTestAttribute>()
                .WithoutMemberAttribute<TestMemberMarkerAttribute>();
            });
        }
    }

    [Generator]
    public class WithEverythingTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithEverythingTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithAttribute<TestClassMarkerAttribute>()
                .WithInterface<ITestInterFace>()
                .WithBaseClass<TestBaseClass>()
                .WithMemberAttribute<TestMemberMarkerAttribute>();
            });
        }
    }

    [Generator]
    public class WithoutAnythingTestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.CodeTemplate(x =>
            {
                x.SetCodeTemplate("WithoutAnythingTestTemplate", code =>
                {
                    var codeString = SourceCode.CodeToGenerate(code.Namespace!, code.Class!);
                    return codeString;
                })
                .WithoutAttribute<IgnoreInTestAttribute>()
                .WithoutAttribute<TestClassMarkerAttribute>()
                .WithoutInterface<ITestInterFace>()
                .WithoutBaseClass<TestBaseClass>()
                .WithoutMemberAttribute<TestMemberMarkerAttribute>();
            });
        }
    }
}

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
}