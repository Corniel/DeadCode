using DeadCode.Syntax;

namespace DeadCode;

partial class CSharpCodeBaseResolver : CSharpSyntaxWalker
{
    public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        CodeBase.LinkMember(node, Model, Document);
        base.VisitEnumMemberDeclaration(node);
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
}
