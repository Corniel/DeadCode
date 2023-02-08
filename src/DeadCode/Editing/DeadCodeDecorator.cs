using DeadCode.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.IO;
using System.Threading.Tasks;

namespace DeadCode.Editing;

public static class DeadCodeDecorator
{
    public static AttributeSyntax Attribute()
    {
        var name = SyntaxFactory.ParseName("Obsolete");
        var arguments = SyntaxFactory.ParseAttributeArgumentList("(\"This code is not used and should be removed.\")");
        return SyntaxFactory.Attribute(name, arguments);
    }

    public static async Task Change(CodeBase codeBase)
    {
        foreach (var code in codeBase.Code)
        {
            if (code.IsDead) 
            {
                var editor = await DocumentEditor.CreateAsync(code.Document);
                editor.InsertBefore(code.Node!, Attribute());
                var updated = editor.GetChangedDocument();

                var newContent = (await updated.GetSyntaxTreeAsync())?
                    .GetCompilationUnitRoot()
                    .NormalizeWhitespace()
                    .GetText()
                    .ToString();
                    
                using var writer = new StreamWriter(updated.FilePath!);
                writer.Write(newContent);
            }
        }
    }

    
}
