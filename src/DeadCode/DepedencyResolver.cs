using DeadCode.Syntax;

namespace DeadCode;

public static class DepedencyResolver
{
    public static bool IsEntryPoint(ISymbol symbol)
        => symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "EntryPoint" || a.AttributeClass?.Name == "EntryPointAttribute")
        || (symbol is IMethodSymbol method && IsEntryPoint(method));

    public static bool IsEntryPoint(IMethodSymbol method) 
        => IsProgramMain(method)
        || IsHttpMethod(method);

    public static bool IsHttpMethod(IMethodSymbol method)
        => method
        .GetAttributes()
        .Any(a => a.AttributeClass.IsAssignableTo(SystemType.Microsoft_AspNetCore_Mvc_Routing_HttpMethodAttribute));


    public static bool IsProgramMain(IMethodSymbol method) 
        => method.Name == "Main"
        && method.ContainingType.Name == "Program";
}
