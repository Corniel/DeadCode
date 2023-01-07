using System.Collections.Generic;
using System.Diagnostics;

namespace DeadCode;

[DebuggerDisplay("{Symbol}, References: {References.Count}")]
public sealed class Code
{
    public Code(ISymbol symbol, params ISymbol[] references)
    {
        Symbol = symbol;
        References = new HashSet<ISymbol>(references, SymbolEqualityComparer.Default);
    }

    public bool Used { get; private set; }

    public SyntaxNode? Node { get; internal set; }

    public ISymbol Symbol { get; }

    public ISet<ISymbol> References { get; }
}

