namespace DeadCode;

[DebuggerDisplay("Count = {Count}")]
[DebuggerVisualizer(typeof(Diagnostics.CollectionDebugView))]
public sealed class References : IReadOnlyCollection<ISymbol>
{
    private readonly HashSet<ISymbol> Set = new(SymbolEqualityComparer.Default);

    /// <inheritdoc />
    public int Count => Set.Count;

    public bool Add(ISymbol symbol) 
        => symbol.HasSource() 
        && Set.Add(symbol);

    /// <inheritdoc />
    [Pure]

    public IEnumerator<ISymbol> GetEnumerator() => Set.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
