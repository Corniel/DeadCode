using DeadCode.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.IO;
using System.Threading.Tasks;

namespace DeadCode.Editing;

public static class DeadCodeRemover
{
    public static async Task Change(CodeBase codeBase)
    {
        foreach (var code in codeBase.Code)
        {
            if (code.IsDead) 
            {
                var editor = await DocumentEditor.CreateAsync(code.Document);
                editor.RemoveNode(code.Node!);
                var updated = editor.GetChangedDocument();

                if( updated is { } && await updated.ContainsCode())
                {
                    var newContent = (await updated.GetSyntaxTreeAsync())?
                        .GetCompilationUnitRoot()
                        .NormalizeWhitespace()
                        .GetText()
                        .ToString();
                    
                    using var writer = new StreamWriter(updated.FilePath!);
                    writer.Write(newContent);
                }
                else
                {
                    File.Delete(code.Document!.FilePath!);
                }
            }
        }
    }
}
