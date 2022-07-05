using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SourceGenLib
{
    //https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/#1-creating-the-source-generator-project
    [Generator]
    public class EnumGen : IIncrementalGenerator
    {
        private const string fullName = "SourceGenLib.EnumExtensionsAttribute";
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Add the marker attribute to the compilation
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "EnumExtensionsAttribute.g.cs",
                SourceText.From(SourceGenHelper.Attribute, Encoding.UTF8)));

            // Do a simple filter for enums
            IncrementalValuesProvider<EnumDeclarationSyntax> enumDeclarations
                = context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticsTargetForGeneration(ctx))
                .Where(m => m is not null)!;

            //Combine the found enums with the "compilation" //not really clear on what this means?
            IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> compilatedEnums
                = context.CompilationProvider.Combine(enumDeclarations.Collect());



            //Generate the source using the compilation and enums
            context.RegisterSourceOutput(compilatedEnums, static (sourceProductionContext, source)
                => Execute(source.Item1, source.Item2, sourceProductionContext));
        }

        private static void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext sourceProductionContext)
        {
            if (enums.IsDefaultOrEmpty) return; // the attribute has not been used yet so nothing to do.

            IEnumerable<EnumDeclarationSyntax> enumDeclarations = enums.Distinct();

            //extract needed information
            List<EnumToGen> enumToGenerate = GetTypesToGenerate(compilation, enumDeclarations, sourceProductionContext.CancellationToken);

            if (enumToGenerate.Count > 0)
            {
                var result = SourceGenHelper.GenerateExtensionClass(enumToGenerate);
                sourceProductionContext.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private static List<EnumToGen> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken cancellationToken)
        {
            //list to hold oputput
            var enumsToGenerate = new List<EnumToGen>();

            //get the smenatic representation of the marker attribute
            INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(fullName);
            if (enumAttribute == null)
            {
                //if this is null the marker attribute type was not found
                // could indicate there was an error
                return enumsToGenerate;
            }

            foreach (EnumDeclarationSyntax enumDeclarationSyntax in enums)
            {
                //bail out it cancellation token was cancelled
                cancellationToken.ThrowIfCancellationRequested();

                //get the semantic representation of the enum syntax
                SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);


                if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                {
                    // something went wrong move to the next decalaration syntax
                    continue;
                }

                //get fully qualified naem of the enum
                var fullEnumName = enumSymbol.ToString();
                string? className = "EnumExtensions";

                foreach (AttributeData attributeData in enumSymbol.GetAttributes())
                {

                    if (!enumAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        continue;
                    }

                    if (!attributeData.ConstructorArguments.IsEmpty)
                    {
                        ImmutableArray<TypedConstant> args = attributeData.ConstructorArguments;
                        className = (string)args[0].Value!;
                    }

                    foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
                    {
                        if (namedArgument.Key == "ExtensionClassName"
                            && namedArgument.Value.Value?.ToString() is { } n)
                        {
                            className = n;
                        }
                    }
                    break;
                }

                //get all member in the enum
                ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
                var members = new List<string>(enumMembers.Length);

                //get all the fields from the enum and  add their name to the list
                foreach (ISymbol member in enumMembers)
                {
                    if (member is IFieldSymbol fieldSymbol && fieldSymbol.ConstantValue is not null)
                    {
                        members.Add(member.Name);
                    }
                }



                enumsToGenerate.Add(new EnumToGen(fullEnumName, members, className!));
            }


            return enumsToGenerate;
        }

        private static EnumDeclarationSyntax? GetSemanticsTargetForGeneration(GeneratorSyntaxContext ctx)
        {
            //we know the type is EnumDeclarationSyntex from previous method call
            var enumDeclarationSyntax = (EnumDeclarationSyntax)ctx.Node;

            // itterate through all the attributes on  the method
            foreach (AttributeListSyntax attributeListSyntax in enumDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        //for some reason the symbol could not be retrieved, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = (INamedTypeSymbol)attributeSymbol.ContainingSymbol;
                    string fullname = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullname == fullName)
                    {
                        return enumDeclarationSyntax;
                    }
                }
            }
            return null;
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode s)
        {
            return s is EnumDeclarationSyntax eds && eds.AttributeLists.Count > 0;
        }
    }

    public readonly struct EnumToGen
    {
        public readonly string Name;
        public readonly List<string> Values;
        public readonly string ClassName;
        public EnumToGen(string n, List<string> v, string cl_n)
        {
            Name = n;
            Values = v;
            ClassName = cl_n;
        }

    }

    public static class SourceGenHelper
    {
        public const string Attribute = @"namespace SourceGenLib
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
        public EnumExtensionsAttribute(string extensionClassName)
        {
            ExtensionClassName = extensionClassName;
        }

        public string ExtensionClassName { get; set; }
    }
}";

        public static string GenerateExtensionClass(List<EnumToGen> enumsToGenerate)
        {
            var sb = new StringBuilder();
            foreach (var enumToGenerate in enumsToGenerate)
            {
                sb.Append(@"
namespace SourceGenLib{");

                sb.Append(@"
    public static partial class ");
                sb.Append(enumToGenerate.ClassName);
                sb.Append(@" {
        public static string ToStringFast(this ").Append(enumToGenerate.Name).Append(@" value)
                    => value switch
                    {");
                foreach (var member in enumToGenerate.Values)
                {
                    sb.Append(@"
                ").Append(enumToGenerate.Name).Append('.').Append(member)
                        .Append(" => nameof(")
                        .Append(enumToGenerate.Name).Append('.').Append(member).Append("),");
                }

                sb.Append(@"
                    _ => value.ToString(),
                };

    }");


                sb.Append(@"
}");
            }
            return sb.ToString();
        }
    }
}


