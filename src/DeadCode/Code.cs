using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DeadCode;

public class Code
{
    public Code(ISymbol symbol, params ISymbol[] references)
    {
        Symbol = symbol;
        References = new HashSet<ISymbol>(references, SymbolEqualityComparer.Default);
    }

    public bool Used { get; internal set; }

    public SyntaxToken? Identifier { get; internal set; }

    public ISymbol Symbol { get; }

    public ISet<ISymbol> References { get; }
}

