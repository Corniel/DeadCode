namespace DeadCode;

public class DepedencyResolver : CSharpSyntaxWalker
{
    public DepedencyResolver(CodeBase codeBase)
    {
        CodeBase = codeBase;
    }

    public CodeBase CodeBase { get; }

    public SemanticModel? Model { get; private set; }

    public void Resolve(SyntaxNode node, SemanticModel model)
    {
        Model = Guard.NotNull(model, nameof(model));
        Visit(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } type)
        {
            CodeBase.SetNode(type, node);
        }
        base.VisitClassDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } type)
        {
            CodeBase.SetNode(type, node);
        }
        base.VisitRecordDeclaration(node);
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } ctor)
        {
            CodeBase.SetNode(ctor, node);
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } method)
        {
            CodeBase.SetNode(method, node);
        }
        base.VisitMethodDeclaration(node);
    }


    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } property)
        {
            CodeBase.SetNode(property, node);
        }
        base.VisitPropertyDeclaration(node);
    }
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (CodeBase.Parent(node) is { } parent
            && Model.GetSymbolInfo(node).Symbol is { } symbol)
        {
            parent.References.Add(symbol);
        }
        base.VisitIdentifierName(node);
    }
}
