using DeadCode.Syntax;

namespace DeadCode;

public sealed partial class CSharpCodeBaseResolver : CSharpSyntaxWalker
{
    public CSharpCodeBaseResolver(CodeBase codeBase, Document document, SemanticModel model)
    {
        CodeBase = codeBase;
        Document = document;
        Model = model;
    }

    private readonly CodeBase CodeBase;
    private readonly SemanticModel Model;
    private readonly Document Document;

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        var name = node.Parent is ObjectCreationExpressionSyntax create ? (SyntaxNode)create : node;
        CodeBase.LinkReference(name, Model, Document);
        base.VisitIdentifierName(node);
    }
}
