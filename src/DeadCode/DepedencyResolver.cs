using DeadCode.Syntax;

namespace DeadCode;

public class DepedencyResolver
{
    public virtual bool IsEntryPoint(IMethodSymbol method)
    {
        return IsProgramMain(method)
            || IsHttpMethod(method);
    }

    protected virtual bool IsHttpMethod(IMethodSymbol method)
        => method
        .GetAttributes()
        .Any(a => a.AttributeClass.IsAssignableTo(SystemType.Microsoft_AspNetCore_Mvc_Routing_HttpMethodAttribute));
        

    private static bool IsProgramMain(IMethodSymbol method)
    {
        return method.Name == "Main" && method.ContainingType.Name == "Program";
    }
}
