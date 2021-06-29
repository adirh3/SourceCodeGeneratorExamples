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
    public class TryCatchCodeGenerator : ISourceGenerator
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
                        tds.AttributeLists.Any(at => at.ToString().StartsWith("[TryCatch")));

                foreach (TypeDeclarationSyntax typeDeclarationSyntax in convertStaticClasses)
                {
                    string className = typeDeclarationSyntax.Identifier.ToString();
                    SyntaxNode parentNode = typeDeclarationSyntax.Parent;
                    if (parentNode is not NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                        continue;

                    IEnumerable<MethodDeclarationSyntax> methods =
                        typeDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>();

                    // Get the namespace's parent which is the using
                    string classCode = namespaceDeclarationSyntax.Parent?.ToString()
                        .Replace("[TryCatch]", string.Empty);

                    foreach (MethodDeclarationSyntax methodDeclarationSyntax in methods)
                    {
                        string methodName = methodDeclarationSyntax.Identifier.ToString();
                        string methodDeclaration = methodDeclarationSyntax.ToString();
                        string methodBody = methodDeclarationSyntax.Body!.ToString();
                        string tryCatchMethod = methodDeclaration.Replace(methodName, "Try" + methodName)
                            .Replace(methodBody, $@"{{try {methodBody} catch(Exception){{ return default; }}}}");
                        classCode = classCode!.Replace(methodDeclaration, tryCatchMethod);
                    }

                    context.AddSource("StaticGenerator" + className,
                        SourceText.From(classCode!, Encoding.UTF8));
                }
            }
        }
    }
}