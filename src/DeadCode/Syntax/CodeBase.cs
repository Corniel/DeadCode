namespace DeadCode.Syntax;

public sealed class CodeBase
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<ISymbol, Code> lookup = new(SymbolEqualityComparer.Default);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<SyntaxNode, Code> nodes = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly object locker = new();

    public Code this[ISymbol symbol] => lookup[symbol];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IReadOnlyCollection<ISymbol> Symbols => lookup.Keys;

    public IReadOnlyCollection<Code> Code => lookup.Values;

    public bool FullyResolved
        => Code.All(c => c.Node is { })
        && Code.Any(c => c.IsEntryPoint);


    public Code? GetOrCreate(ISymbol symbol)
    {
        if (symbol.Is(SystemType.System_Void)
            || symbol.Kind == SymbolKind.Discard)
        {
            return null;
        }
        else
        {
            lock (locker)
            {
                if (!lookup.TryGetValue(symbol, out var code))
                {
                    code = new(symbol);
                    lookup[symbol] = code;
                }
                return code;
            }
        }
    }

    private Code GetOrCreate(ISymbol symbol, SyntaxNode node, Document document)
    {
        var code = GetOrCreate(symbol)!;
        code.Node = node;
        code.Document = document;
        nodes[node] = code;
        return code;
    }

    public Code? Parent(SyntaxNode node)
    {
        return node.Ancestors().Select(Find).FirstOrDefault(c => c is { });
        Code? Find(SyntaxNode node) => nodes.TryGetValue(node, out var code) ? code : null;
    }

    public Code SetNode(INamedTypeSymbol type, SyntaxNode node, Document document)
        => GetOrCreate(type, node, document);

    public Code SetNode(IMethodSymbol method, SyntaxNode node, Document document)
    {
        var code = GetOrCreate(method, node, document);
        GetOrCreate(method.ContainingType)!.UsedBy.Add(code);
        GetOrCreate(method.ReturnType)?.UsedBy.Add(code);

        foreach (var type in method.TypeArguments.Where(t => t.TypeKind != TypeKind.TypeParameter))
        {
            GetOrCreate(type)!.UsedBy.Add(code);
        }
        return code;
    }

    public Code SetNode(IPropertySymbol prop, SyntaxNode node, Document document)
    {
        var code = GetOrCreate(prop, node, document);
        GetOrCreate(prop.ContainingType)!.UsedBy?.Add(code);
        GetOrCreate(prop.Type)?.UsedBy?.Add(code);
        return code;
    }
}
