using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis;

internal static class DeadCodeDocumentExtenions
{
    public static async Task<bool> ContainsCode(this Document document)
        => (await document.GetSyntaxRootAsync())!
        .DescendantNodes()
        .Any(d => d.IsAnyKind(
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.EnumDeclaration));
}
