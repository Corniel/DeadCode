namespace Microsoft.CodeAnalysis;

public static class DeadCodeSymbolExtensions
{
    public static bool HasSource(this ISymbol? symbol) => symbol switch
    {
        null => false,
        INamedTypeSymbol x => x.ContainingAssembly.HasSource(),
        IPropertySymbol x => x.ContainingAssembly.HasSource(),
        IMethodSymbol x => x.ContainingAssembly.HasSource(),
        IFieldSymbol x => x.ContainingAssembly.HasSource(),
        IAssemblySymbol x => x.HasSource(),
        _ => false,
    };

    private static bool HasSource(this IAssemblySymbol assembly) => assembly.GetType().Name != "NonSourceAssemblySymbol";


}
