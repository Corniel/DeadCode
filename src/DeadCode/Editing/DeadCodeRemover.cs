using DeadCode.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Threading.Tasks;

namespace DeadCode.Editing;

public sealed class DeadCodeRemover
{
    public async Task Change(CodeBase codeBase, IReadOnlyDictionary<string, Document> documents)
    {
        foreach(var code in codeBase.Code.Where(c => c.IsDead))
        {
            if(documents.TryGetValue(code.Node!.SyntaxTree.FilePath, out var document)) 
            {
                var editor = await DocumentEditor.CreateAsync(document);
                editor.RemoveNode(code.Node!);
            }
            //var doc = code.Node.
            //var document = code.Node.GetDo
        }
    }
}
