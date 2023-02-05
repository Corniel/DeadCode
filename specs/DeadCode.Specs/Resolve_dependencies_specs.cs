using CodeAnalysis.TestTools;
using DeadCode;
using FluentAssertions;
using Specs.Analyzers;
using Specs.Tooling;

namespace Resolve_dependencies_specs;

public class Ctor
{
    [Test]
    public void Depends_on_base()
    {
        var analyzer = new DefaultAnalyzer();

        _ = analyzer.ForCS()
        .AddSnippet(@"
            
    public class MyClass
    {
        public MyClass() { }
        public MyClass(MyClass other) : this(){ }
    }")
        .ReportIssues();

        analyzer.CodeBase.Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Refs("MyClass.MyClass()", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass()"] = Symbol.Refs("MyClass", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass(MyClass)"] = Symbol.Refs(),
        });
    }
}

static class Dependencies
{
    public static CodeBase Resolve(string snippet)
    {
        var analyzer = new DefaultAnalyzer();

        _ = analyzer.ForCS()
            .AddSnippet(snippet)
            .WithLanguageVersion(LanguageVersion.CSharp11)
            .ReportIssues();

        return analyzer.CodeBase;
    }
}