namespace DeadCode;

internal sealed class CSharpDepedencyResolver : CSharpSyntaxWalker
{
    public CSharpDepedencyResolver(DepedencyResolver resolver, SemanticModel model)
    {
        Resolver = resolver;
        Model = model;
    }

    private readonly DepedencyResolver Resolver;
    private readonly SemanticModel Model;
    private CodeBase CodeBase => Resolver.CodeBase;

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
                CodeBase.GetOrCreate(init)?.UsedBy.Add(code);
            }

            // default ctor.
            if (!ctor.Parameters.Any())
            {
                CodeBase.GetOrCreate(ctor)!.UsedBy.Add(CodeBase.GetOrCreate(ctor.ContainingType));
            }
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } method)
        {
            var code = CodeBase.SetNode(method, node);
            code.IsEntryPoint = Resolver.IsEntryPoint(method);
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
            && Model!.GetSymbolInfo(name).Symbol is { } symbol)
        {
            CodeBase.GetOrCreate(symbol)?.UsedBy.Add(parent);
        }
        base.VisitIdentifierName(node);
    }
}

