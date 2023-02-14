using DeadCode.Syntax;

namespace DeadCode;

partial class CSharpCodeBaseResolver : CSharpSyntaxWalker
{
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitClassDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitEnumDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        CodeBase.LinkType(node, Model, Document);
        base.VisitInterfaceDeclaration(node);
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
}
