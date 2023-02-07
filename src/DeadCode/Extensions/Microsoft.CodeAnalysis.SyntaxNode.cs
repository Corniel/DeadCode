namespace Microsoft.CodeAnalysis;

public static class DeadCodeSyntaxNodeExtensions
{
    [Pure]
    public static bool IsAnyKind(this SyntaxNode node, params SyntaxKind[] kinds)
        => kinds.Any(node.IsKind);
}
