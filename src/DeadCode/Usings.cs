namespace DeadCode;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(Diagnostics.CollectionDebugView))]
public sealed class Usings : IReadOnlyCollection<Code>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly HashSet<Code> Set = new();

    /// <inheritdoc />
    public int Count => Set.Count;

    public bool Add(Code? code) 
        => code is { }
        && Set.Add(code);

    /// <inheritdoc />
    [Pure]

    public IEnumerator<Code> GetEnumerator() => Set.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
