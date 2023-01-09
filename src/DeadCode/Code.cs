namespace DeadCode;

[DebuggerDisplay("{Symbol}, References: {References.Count}")]
public sealed class Code
{
    public Code(ISymbol symbol, params ISymbol[] references)
    {
        Symbol = symbol;
    }

    public bool Used { get; private set; }

    public SyntaxNode? Node { get; internal set; }

    public ISymbol Symbol { get; }

    public References References { get; } = new();
}

