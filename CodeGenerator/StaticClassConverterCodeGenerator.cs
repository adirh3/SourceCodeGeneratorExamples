using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeGenerator
{
    [Generator]
    public class StaticClassConverterCodeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
            {
                IEnumerable<TypeDeclarationSyntax> convertStaticClasses = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<TypeDeclarationSyntax>().Where(tds =>
                        tds.AttributeLists.Any(at => at.ToString().StartsWith("[ConvertStatic")));

                foreach (TypeDeclarationSyntax typeDeclarationSyntax in convertStaticClasses)
                {
                    string className = typeDeclarationSyntax.Identifier.ToString();
                    SyntaxNode parentNode = typeDeclarationSyntax.Parent;
                    if (parentNode is not NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                        continue;

                    // Get the namespace's parent which is the using
                    string generatedNonStaticClass = namespaceDeclarationSyntax.Parent?.ToString()
                        .Replace("[ConvertStatic]", string.Empty)
                        .Replace("namespace " + namespaceDeclarationSyntax.Name,
                            "namespace " + namespaceDeclarationSyntax.Name + "Generated")
                        .Replace("static ", string.Empty).Replace(className, className + "NonStatic");
                    context.AddSource("StaticGenerator" + className,
                        SourceText.From(generatedNonStaticClass!, Encoding.UTF8));
                }
            }
        }
    }
}