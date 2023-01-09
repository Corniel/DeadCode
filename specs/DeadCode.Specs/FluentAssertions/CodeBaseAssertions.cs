using DeadCode;
using FluentAssertions.Execution;
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
        var issues = 0;
        var report = new StringBuilder();

        foreach (var kvp in depedencies)
        {
            if (Subject.Symbols.FirstOrDefault(s => kvp.Key.Equals(s)) is { } symbol)
            {
                var code = Subject[symbol];

                var missing = kvp.Value.Where(s => !code.References.Any(r => s.Equals(r))).ToList();
                var extra = code.References.Where(r => !kvp.Value.Any(s => s.Equals(r))).ToList();

                if (missing.Any() || extra.Any())
                {
                    report.AppendLine($"[x] {symbol}");
                    if (missing.Any()) report.AppendLine($"    - {string.Join(", ", missing)}");
                    if (extra.Any()) report.AppendLine($"    + {string.Join(", ", extra)}");

                    issues++;
                }
                else
                {
                    report.AppendLine($"[ ] {symbol}");
                }
            }
            else
            {
                report.AppendLine($"[-] {kvp.Key}");
                issues++;
            }
        }
        foreach(var code in Subject.Code)
        {
            if (!depedencies.Keys.Any(s => s.Equals(code.Symbol)))
            {
                report.AppendLine($"[+] {code.Symbol}");
                issues++;
            }
        }

        Execute.Assertion
            .ForCondition(issues == 0)
            .FailWith(report.ToString());

        Console.WriteLine(report);

        return new(this);
    }
}
