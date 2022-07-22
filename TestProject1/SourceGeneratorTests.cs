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

        }
        ///Without attribute
        [TestMethod]
        public void WithoutAttributeTest()
        {

        }
        ///With base class
        [TestMethod]
        public void WithBaseClassTest()
        {

        }
        ///Without base class
        [TestMethod]
        public void WithoutBaseClassTest()
        {

        }
        ///With interface
        [TestMethod]
        public void WithInterfaceTest()
        {

        }
        ///Without interface
        [TestMethod]
        public void WithoutInterfaceTest()
        {

        }
        ///With member atrribute
        [TestMethod]
        public void WithMemberAttributeTest()
        {

        }
        ///Without member attribute
        [TestMethod]
        public void WithoutMemberAttributeTest()
        {

        }
        ///With everything
        [TestMethod]
        public void WithEverythingTest()
        {

        }
        ///Without anything
        [TestMethod]
        public void WithoutAnythingTest()
        {

        }


        public static void RunTest<T>(IEnumerable<SyntaxTree> sourceCode,
            out Compilation outputCompilation,
            out ImmutableArray<Diagnostic> diagnostics,
            out GeneratorDriverRunResult driverResult)
            where T : IIncrementalGenerator
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var inputCompilation = CSharpCompilation.Create("compilation", sourceCode, null, compilationOptions);
            var generator = (T)Activator.CreateInstance(typeof(T))!;

            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out outputCompilation, out diagnostics);

            driverResult = driver.GetRunResult();
        }
    }
}
