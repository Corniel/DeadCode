using DeadCode.Syntax;

namespace DeadCode;

public static class DepedencyResolver
{
    public static bool IsEntryPoint(IMethodSymbol method)
    {
        return IsProgramMain(method)
            || IsHttpMethod(method);
    }

    public static bool IsHttpMethod(IMethodSymbol method)
        => method
        .GetAttributes()
        .Any(a => a.AttributeClass.IsAssignableTo(SystemType.Microsoft_AspNetCore_Mvc_Routing_HttpMethodAttribute));


    public static bool IsProgramMain(IMethodSymbol method)
    {
        return method.Name == "Main" && method.ContainingType.Name == "Program";
    }
}
