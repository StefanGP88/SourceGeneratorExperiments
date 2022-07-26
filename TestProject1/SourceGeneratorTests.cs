using FluentAssertions;
using FluentSyntaxGenerator.Unittest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TestProject1
{
    [TestClass]
    public class SourceGeneratorTests
    {
        ///Tests to do
        ///With attribute
        [TestMethod]
        public void WithAttributeTest()
        {
            RunTest<WithAttributeTestGenerator>(SourceCode.WithAttribute,  out var result);
            EvaluateResult(result, SourceCode.WithAttributeExpectedResult);
        }
        ///Without attribute
        [TestMethod]
        public void WithoutAttributeTest()
        {
            RunTest<WithoutAttributeTestGenerator>(SourceCode.WithoutAttribute, out var result);
            EvaluateResult(result, SourceCode.WithoutAttributeExpectedResult);
        }
        ///With base class
        [TestMethod]
        public void WithBaseClassTest()
        {
            RunTest<WithBaseClassTestGenerator>(SourceCode.WithBaseClass, out var result);
            EvaluateResult(result, SourceCode.WithBaseClassExpectedResult);
        }
        ///Without base class
        [TestMethod]
        public void WithoutBaseClassTest()
        {
            RunTest<WithoutBaseClassTestGenerator>(SourceCode.WithoutBaseClass, out var result);
            EvaluateResult(result, SourceCode.WithoutBaseClassExpectedResult);
        }
        ///With interface
        [TestMethod]
        public void WithInterfaceTest()
        {
            RunTest<WithInterfaceTestGenerator>(SourceCode.WithInterface, out var result);
            EvaluateResult(result, SourceCode.WithInterfaceExpectedResult);
        }
        ///Without interface
        [TestMethod]
        public void WithoutInterfaceTest()
        {
            RunTest<WithoutInterfaceTestGenerator>(SourceCode.WithoutInterface, out var result);
            EvaluateResult(result, SourceCode.WithoutInterfaceExpectedResult);
        }
        ///With member atrribute
        [TestMethod]
        public void WithMemberAttributeTest()
        {
            RunTest<WithMemberAttributeTestGenerator>(SourceCode.WithMemberAttribute, out var result);
            EvaluateResult(result, SourceCode.WithMemberAttributeExpectedResult);
        }
        ///Without member attribute
        [TestMethod]
        public void WithoutMemberAttributeTest()
        {
            RunTest<WithoutMemberAttributeTestGenerator>(SourceCode.WithoutMemberAttribute, out var result);
            EvaluateResult(result, SourceCode.WithoutMemberAttributeExpectedResult);
        }
        ///With everything
        [TestMethod]
        public void WithEverythingTest()
        {
            RunTest<WithEverythingTestGenerator>(SourceCode.WithEverything, out var result);
            EvaluateResult(result, SourceCode.WithEverythingExpectedResult);
        }
        ///Without anything
        [TestMethod]
        public void WithoutAnythingTest()
        {
            RunTest<WithoutAnythingTestGenerator>(SourceCode.WithoutAnything, out var result);
            EvaluateResult(result, SourceCode.WithoutAnythingExpectedResult);
        }

        public static void RunTest<T>(SyntaxTree syntaxTree,
            out GeneratorDriverRunResult driverResult)
            where T : IIncrementalGenerator
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var sourceCode =  new[] { SourceCode.Markers, syntaxTree };
            var inputCompilation = CSharpCompilation.Create("compilation", sourceCode, null, compilationOptions);
            var generator = (T)Activator.CreateInstance(typeof(T))!;

            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _);

            driverResult = driver.GetRunResult();
        }

        public static void EvaluateResult(GeneratorDriverRunResult result, SyntaxTree expectedResult)
        {
            result.Results.Should().HaveCount(1);
            result.Results[0].GeneratedSources.Should().HaveCount(1);

            var normalizedResult = result.Results[0].GeneratedSources[0].SyntaxTree.GetRoot().NormalizeWhitespace().ToFullString();
            var normalizedExpectation = expectedResult.GetRoot().NormalizeWhitespace().ToFullString();

            normalizedResult.Should().BeEquivalentTo(normalizedExpectation);
        }
    }
}
