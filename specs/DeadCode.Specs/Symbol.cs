using System.Diagnostics.CodeAnalysis;

namespace Specs;

public readonly struct Symbol : IEquatable<ISymbol>
{
    public readonly string Name;

    public Symbol(string name) => Name = name;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ISymbol other && Equals(other);

    public bool Equals(ISymbol? other)
    {
        var str = other?.ToString() ?? string.Empty;
        return str.Equals(Name);
    }

    /// <inheritdoc />
    public override string ToString() => Name;

    public static implicit operator Symbol(string name) => new(name);

    public static Symbol[] Refs(params string[] symbols) 
        => symbols.Any()
        ? symbols.Select(name => new Symbol(name)).ToArray()
        : Array.Empty<Symbol>();
}
