using CodeAnalysis.TestTools;
using CodeAnalysis.TestTools.Contexts;
using DeadCode.Syntax;

namespace Specs.Tooling;

internal static class Setup
{
    public static CSharpAnalyzerVerifyContext Collector() => new CodeBaseAnalyzer().ForCS();

    public static CodeBase CodeBase(this CSharpAnalyzerVerifyContext context)
    {
        _ = context.ReportIssues();
        return ((CodeBaseAnalyzer)context.Analyzers.First()).CodeBase;
    }
}
