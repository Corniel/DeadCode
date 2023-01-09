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

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } type)
        {
            CodeBase.SetNode(type, node);
        }
        base.VisitStructDeclaration(node);
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
            var code = CodeBase.SetNode(ctor, node);

            if (node.Initializer is { } initializer && Model.GetSymbolInfo(initializer).Symbol is { } init)
            {
                code.References.Add(init);
            }

            // default ctor.
            if (!ctor.Parameters.Any())
            {
                CodeBase.GetOrCreate(ctor.ContainingType).References.Add(ctor);
            }
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
        var name = node.Parent is ObjectCreationExpressionSyntax create ? (SyntaxNode)create : node;

        if (CodeBase.Parent(name) is { } parent
            && Model.GetSymbolInfo(name).Symbol is { } symbol)
        {
            parent.References.Add(symbol);
        }
        base.VisitIdentifierName(node);
    }
}
