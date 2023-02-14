namespace DeadCode.Syntax;

public sealed class CodeBase
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<ISymbol, Code> lookup = new(SymbolEqualityComparer.IncludeNullability);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<SyntaxNode, Code> nodes = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly object locker = new();

    public Code this[ISymbol symbol] => lookup[symbol];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IReadOnlyCollection<ISymbol> Symbols => lookup.Keys;

    public IReadOnlyCollection<Code> Code => lookup.Values;

    public Code? TryGetCode(ISymbol symbol)
    {
        if (symbol.Kind == SymbolKind.Local)
        {
            symbol = symbol.ContainingType;
        }

        return symbol.Is(SystemType.System_Void)
            || symbol.Kind == SymbolKind.Discard
            ? null
            : GetCode(symbol);
    }

    public Code GetCode(ISymbol symbol)
    {
        lock (locker)
        {
            if (lookup.TryGetValue(symbol, out var code))
            {
                return code;
            }
            else
            {
                var type = new Code(symbol);
                lookup[symbol] = type;
                return type;
            }
        }
    }

    public void LinkType(SyntaxNode declaration, SemanticModel model, Document document)
    {
        if (model.GetDeclaredSymbol(declaration) is INamedTypeSymbol symbol)
        {
            GetCode(symbol).Link(declaration, document);
        }
    }

    public Code? LinkMember(SyntaxNode declaration, SemanticModel model, Document document)
    {
        if (model.GetDeclaredSymbol(declaration) is { } symbol)
        {
            var member = GetCode(symbol);
            member.Link(declaration, document);

            var type = GetCode(symbol.ContainingType);
            type.UsedBy.Add(member);

            foreach (var other in Symbols(symbol))
            {
                if (TryGetCode(other) is { } code)
                {
                    code.UsedBy.Add(member);
                }
            }
            return member;
        }
        else
        {
            return null;
        }

        static IEnumerable<ISymbol> Symbols(ISymbol symbol)
        {
            if(symbol is IMethodSymbol method)
            {
                foreach(var param in method.Parameters)
                {
                    yield return param.Type;
                }
                yield return method.ReturnType;
            }
            else if (symbol is IPropertySymbol property)
            {
                yield return property.Type;
            }
            else if (symbol is IFieldSymbol field)
            {
                yield return field.Type;
            }
        }
    }

    public void LinkConstructor(SyntaxNode declaration, SyntaxNode? initializer, SemanticModel model, Document document)
    {
        if (LinkMember(declaration, model, document) is { } ctor)
        {
            // if the ctor references this or base, that ctor is used by this one.
            if (initializer is { } && model.GetSymbolInfo(initializer).Symbol is IMethodSymbol init)
            {
                GetCode(init).UsedBy.Add(ctor);
            }

            // Default constructor is used by its type.
            if (ctor.Symbol.IsDefaultConstructor())
            {
                ctor.UsedBy.Add(GetCode(ctor.Symbol.ContainingType));
            }
        }
    }

    public void LinkReference(SyntaxNode reference, SemanticModel model, Document document)
    {
        if (Parent(reference) is { } parent
            && model.GetSymbolInfo(reference).Symbol is { } symbol
            && TryGetCode(symbol) is { } code)
        {
            code.UsedBy.Add(parent);
        }
    }

    private Code? Parent(SyntaxNode node)
    {
        return node.Ancestors().Select(Find).FirstOrDefault(c => c is { });
        Code? Find(SyntaxNode node) => nodes.TryGetValue(node, out var code) ? code : null;
    }
}
