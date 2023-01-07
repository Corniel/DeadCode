using System.Collections.Generic;
using System.Linq;

namespace DeadCode;

public sealed class CodeBase
{
    private readonly Dictionary<ISymbol, Code> lookup = new Dictionary<ISymbol, Code>(SymbolEqualityComparer.Default);
    private readonly Dictionary<SyntaxNode, Code> nodes = new();
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

    public Code? Parent(SyntaxNode node)
    {
        return node.Ancestors().Select(Find).FirstOrDefault(c => c is { });
        Code? Find(SyntaxNode node) => nodes.TryGetValue(node, out var code) ? code : null;
    }

    public Code SetNode(INamedTypeSymbol type, SyntaxNode node)
    {
        var code = GetOrCreate(type);
        code.Node = node;
        nodes[node] = code;
        return code;
    }

    public Code SetNode(IMethodSymbol method, SyntaxNode node)
    {
        var code = GetOrCreate(method);
        code.Node = node;
        nodes[node] = code;
        code.References.Add(method.ContainingType);
        code.References.Add(method.ReturnType);
        foreach(var type in method.TypeArguments)
        {
            code.References.Add(type);
        }
        return code;
    }

    public Code SetNode(IPropertySymbol prop, SyntaxNode node)
    {
        var code = GetOrCreate(prop);
        code.Node = node;
        nodes[node] = code;
        code.References.Add(prop.ContainingType);
        code.References.Add(prop.Type);
        return code;
    }
}
