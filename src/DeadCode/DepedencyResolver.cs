namespace DeadCode;

public class DepedencyResolver
{
    public DepedencyResolver(CodeBase codeBase)
    {
        CodeBase = codeBase;
    }

    public CodeBase CodeBase { get; }

    public void Resolve(SyntaxNode node, SemanticModel model)
    {
        var resolver = new CSharpDepedencyResolver(this, model);
        resolver.Visit(node);
    }

    public virtual bool IsEntryPoint(IMethodSymbol method)
    {
        return method.Name == "Main" && method.ContainingType.Name == "Program";
    }
}
