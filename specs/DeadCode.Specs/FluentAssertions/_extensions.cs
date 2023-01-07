using DeadCode;

namespace FluentAssertions;

public static class FluentAsserionExtensions
{
    public static CodeBaseAssertions Should(this CodeBase codebase) => new CodeBaseAssertions(codebase);
}
