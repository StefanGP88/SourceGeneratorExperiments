using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SourceGenLib.Extensions
{
    public class BaseClassFinder
    {
        private IncrementalGeneratorInitializationContext _context;
        private IncrementalValuesProvider<ClassDeclarationSyntax> _classDeclarations;
        private IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> _compilatedClasses;

        public BaseClassFinder(IncrementalGeneratorInitializationContext context)
        {
            _context = context;

            _classDeclarations = _context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, cancelToken) =>
                {
                    return syntaxNode is ClassDeclarationSyntax cds && cds.BaseList != null;
                },
                transform: (ctx, cancelToken) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;
                    return classDeclarationSyntax;
                })
                .Where(provider => provider != null);

            _compilatedClasses = _context.CompilationProvider.Combine(_classDeclarations.Collect());


            var asdfg = _context.CompilationProvider;
        }
        public void BuildFromClassBaseClass<T>(Func<MyClassInfo, string> sourceBuilder)
        {
            _context.RegisterSourceOutput(_compilatedClasses, (sourceProductionContext, source) =>
            {
                var compilation = source.Item1;
                var classes = source.Item2;

                if (classes.IsDefaultOrEmpty) return;

                IEnumerable<ClassDeclarationSyntax> classDeclarations = classes.Distinct();

                var classToGenerate = GetTypesToGenerate<T>(compilation, classDeclarations, sourceProductionContext.CancellationToken);

                foreach (var myClassInfo in classToGenerate)
                {
                    var generatedCode = sourceBuilder(myClassInfo);
                    var generatedFileName = myClassInfo.ClassName + ".g.cs";

                    sourceProductionContext.AddSource(generatedFileName, SourceText.From(generatedCode, Encoding.UTF8));
                }
            });
        }
        private List<MyClassInfo> GetTypesToGenerate<T>(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, CancellationToken cancellationToken)
        {
            var result = new List<MyClassInfo>();
            var type = typeof(T);

            foreach (ClassDeclarationSyntax classDeclarationSyntax in classes)
            {
                var myClassInfo = new MyClassInfo();

                cancellationToken.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                {
                    continue;
                }
                if (classSymbol.BaseType?.ToString() != type.FullName)
                {
                    continue;
                }

                myClassInfo.ClassName = classSymbol.ToString();
                myClassInfo.NameSpace = classSymbol.ContainingNamespace.ToString();
                myClassInfo.Modifiers = classDeclarationSyntax.Modifiers.Select(x => x.Text).ToList();
                myClassInfo.InherritsFrom = classSymbol.BaseType?.ToString();
                myClassInfo.Interfaces = classSymbol.Interfaces.Select(x => x.Name).ToList();


                var afasfladsf = classDeclarationSyntax.Members;

                var prop = classDeclarationSyntax.Members[0];
                var prop_ = prop is PropertyDeclarationSyntax;
                var prop_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is IMethodSymbol;
                var prop_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is IDiscardSymbol;
                var prop_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is IFieldSymbol;
                var prop_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is INamedTypeSymbol;
                var prop_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is IPropertySymbol;
                var prop_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(prop) is ITypeSymbol;

                var ctor = classDeclarationSyntax.Members[1];
                var ctor_ = ctor is ConstructorDeclarationSyntax;
                var ctor_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is IMethodSymbol;
                var ctor_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is IDiscardSymbol;
                var ctor_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is IFieldSymbol;
                var ctor_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is INamedTypeSymbol;
                var ctor_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is IPropertySymbol;
                var ctor_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(ctor) is ITypeSymbol;

                var dtor = classDeclarationSyntax.Members[2];
                var dtor_ = dtor is DestructorDeclarationSyntax;
                var dtor_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IMethodSymbol;
                var dtor_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IDiscardSymbol;
                var dtor_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IFieldSymbol;
                var dtor_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is INamedTypeSymbol;
                var dtor_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IPropertySymbol;
                var dtor_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is ITypeSymbol;

                var meth = classDeclarationSyntax.Members[3];
                var meth_ = meth is MethodDeclarationSyntax;
                var meth_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IMethodSymbol;
                var meth_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IDiscardSymbol;
                var meth_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IFieldSymbol;
                var meth_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is INamedTypeSymbol;
                var meth_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IPropertySymbol;
                var meth_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is ITypeSymbol;

                var enm = classDeclarationSyntax.Members[4];
                var enm_ = enm is EnumDeclarationSyntax;
                var enm_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IMethodSymbol;
                var enm_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IDiscardSymbol;
                var enm_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IFieldSymbol;
                var enm_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is INamedTypeSymbol;
                var enm_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IPropertySymbol;
                var enm_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is ITypeSymbol;


                var field = classDeclarationSyntax.Members[5];
                var field_ = field is FieldDeclarationSyntax;
                var field_ImethodSymbol0 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IMethodSymbol;
                var field_ImethodSymbol1 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IDiscardSymbol;
                var field_ImethodSymbol2 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IFieldSymbol;
                var field_ImethodSymbol3 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is INamedTypeSymbol;
                var field_ImethodSymbol4 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is IPropertySymbol;
                var field_ImethodSymbol5 = compilation.GetSemanticModel(prop.SyntaxTree).GetDeclaredSymbol(dtor) is ITypeSymbol;


                result.Add(myClassInfo);
            }

            return result;
        }
    }
}
