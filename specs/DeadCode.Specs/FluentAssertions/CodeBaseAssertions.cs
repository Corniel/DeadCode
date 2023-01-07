using DeadCode;
using Specs;

namespace FluentAssertions;

public sealed class CodeBaseAssertions
{
    public CodeBaseAssertions(CodeBase subject)
    {
        Subject = subject;
    }

    public CodeBase Subject { get; }

    public AndConstraint<CodeBaseAssertions> HaveDepedencies(Dictionary<Symbol, Symbol[]> depedencies)
    {

        return new(this);
    }
}
