using CodeAnalysis.TestTools;
using DeadCode;
using FluentAssertions;
using Specs.Tooling;

namespace Entry_point_specs;

public class Entry_point_specs
{
    [Test]
    public void Test()
    {
        var analyzer = new CodeBaseResolver();

        _ = analyzer.ForCS()
        .WithOutputKind(OutputKind.ConsoleApplication)
        .AddSnippet(@"
            
    static class Program
    {
        static void Main(params string[] args) {}
    }")
        .ReportIssues();

        analyzer.CodeBase.Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["Program"] = Symbol.Refs("Program.Main(params string[])"),
            ["Program.Main(params string[])"] = Symbol.Refs(),
        });

        analyzer.CodeBase.Code.Should().AllSatisfy(c => c.IsDead.Should().BeFalse(because: c.Symbol.Name));
    }
}
