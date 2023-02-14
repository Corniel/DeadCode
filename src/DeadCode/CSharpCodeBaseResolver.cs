using DeadCode.Syntax;

namespace DeadCode;

public sealed class CSharpCodeBaseResolver : CSharpSyntaxWalker
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

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitClassDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitStructDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitRecordDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        foreach (var declaration in node.Declaration.Variables)
        {
            CodeBase.LinkMember(declaration, Model, Document);
        }
        base.VisitFieldDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        CodeBase.LinkMember(node, Model, Document);
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        CodeBase.LinkMember(node, Model, Document);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        CodeBase.LinkConstructor(node, node.Initializer, Model, Document);
        base.VisitConstructorDeclaration(node);
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        var name = node.Parent is ObjectCreationExpressionSyntax create ? (SyntaxNode)create : node;
        CodeBase.LinkReference(name, Model, Document);
        base.VisitIdentifierName(node);
    }
}
