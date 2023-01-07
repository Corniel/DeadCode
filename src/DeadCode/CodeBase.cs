using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DeadCode;

public sealed class CodeBase
{
    private readonly Dictionary<ISymbol, Code> lookup = new Dictionary<ISymbol, Code>(SymbolEqualityComparer.Default);
    private readonly object locker = new();

    public Code GetOrCreate(ISymbol symbol)
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

    public IReadOnlyCollection<ISymbol> Symbols => lookup.Keys;

    public IReadOnlyCollection<Code> Code => lookup.Values;

    public Dictionary<SyntaxNode, bool> CompilationUnits { get; } = new Dictionary<SyntaxNode, bool>();

    public void SetIdentifier(INamedTypeSymbol type, SyntaxToken identifier)
    {
        GetOrCreate(type).Identifier = identifier;
    }

    public void SetIdentifier(IMethodSymbol method, SyntaxToken identifier)
    {
        var code = GetOrCreate(method);
        code.Identifier = identifier;
        code.References.Add(method.ContainingType);
    }

    public void SetIdentifier(IPropertySymbol prop, SyntaxToken identifier)
    {
        var code = GetOrCreate(prop);
        code.Identifier = identifier;
        code.References.Add(prop.ContainingType);
    }
}
