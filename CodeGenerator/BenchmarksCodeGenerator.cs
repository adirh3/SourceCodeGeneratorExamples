using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace CodeGenerator
{
    [Generator]
    public class BenchmarksCodeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
            {
                IEnumerable<TypeDeclarationSyntax> convertStaticClasses = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<TypeDeclarationSyntax>().Where(tds =>
                        tds.AttributeLists.Any(at => at.ToString().StartsWith("[BenchmarkAttribute")));

                foreach (TypeDeclarationSyntax typeDeclarationSyntax in convertStaticClasses)
                {
                    string className = typeDeclarationSyntax.Identifier.ToString();
                    SyntaxNode parentNode = typeDeclarationSyntax.Parent;
                    if (parentNode is not NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                        continue;

                    // Get the namespace's parent which is the using
                    string classCode = namespaceDeclarationSyntax.Parent?.ToString()
                        .Replace("[BenchmarkAttribute]", string.Empty)
                        .Replace("namespace " + namespaceDeclarationSyntax.Name,
                            "namespace " + namespaceDeclarationSyntax.Name + "Generated")
                        .Replace(className, className + "Benchmarks");;


                    int i = 0;
                    while (TryReplaceFirst(ref classCode, "// Start benchmark",
                        $"var stopWatch{i} = System.Diagnostics.Stopwatch.StartNew();"))
                    {
                        i++;
                    }
                    
                    i = 0;
                    while (TryReplaceFirst(ref classCode, "// End benchmark",
                        $"stopWatch{i}.Stop(); \n System.Console.WriteLine(stopWatch{i}.ElapsedMilliseconds);"))
                    {
                        i++;
                    }


                    context.AddSource("StaticGenerator" + className,
                        SourceText.From(classCode!, Encoding.UTF8));
                }
            }
        }

        public bool TryReplaceFirst(ref string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return false;
            }

            text = text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
            return true;
        }
    }
}