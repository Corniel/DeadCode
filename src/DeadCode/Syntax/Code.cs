namespace DeadCode.Syntax;

[DebuggerDisplay("{Symbol}, Used by: {UsedBy.Count}, IsDead: {IsDead}")]
public class Code
{
    public Code(ISymbol symbol)
    {
        Symbol = symbol;
    }

    /// <summary>The linked syntax node.</summary>
    public SyntaxNode? Node { get; private set; }

    /// <summary>The linked document.</summary>
    public Document? Document { get; private set; }

    /// <summary>The code symbol.</summary>
    public ISymbol Symbol { get; }

    /// <summary>The code that uses this code.</summary>
    public CodeCollection UsedBy { get; } = new();

    /// <summary>True if this code is an entry point.</summary>
    public bool IsEntryPoint { get; internal set; }

    /// <summary>True if this code is no an entry point, neither it is used by any.</summary>
    public bool IsDead
    {
        get
        {
            if (IsEntryPoint 
                || _IsAlive 
                || Node is null 
                || Symbol.IsOverride
                || Symbol.IsImplementation())
            {
                return false;
            }
            else
            {
                _IsAlive = IsAlive(Tracker.Empty);
                return !_IsAlive;
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _IsAlive;

    public void Link(SyntaxNode node, Document document)
    {
        Node = Guard.NotNull(node, nameof(node));
        Document = Guard.NotNull(document, nameof(document));
    }

    [Pure]
    private bool IsAlive(Tracker tracker)
        => tracker.Add(this) is { } added
        && (IsEntryPoint || UsedBy.Any(use => use.IsAlive(added)));

    private sealed class Tracker
    {
        public static readonly Tracker Empty = new(null, null!);

        public Tracker(Tracker? @base, Code code)
        {
            Base = @base;
            Code = code;
        }

        public readonly Tracker? Base;
        public readonly Code Code;

        [Pure]
        public Tracker? Add(Code code)
            => Contains(code)
            ? null
            : new(this, code);

        [Pure]
        public bool Contains(Code code)
            => Code == code
            || Base is { } && Base.Contains(code);
    }
}
