using System.Diagnostics.CodeAnalysis;

namespace Specs;

public readonly struct Symbol : IEquatable<ISymbol>
{
    public readonly string Name;

    public Symbol(string name) => Name = name;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(ISymbol? other)
    {

        return false;
    }

    public static implicit operator Symbol(string name) => new(name);
}
