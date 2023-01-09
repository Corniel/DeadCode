using CodeAnalysis.TestTools;
using DeadCode;
using FluentAssertions;
using Specs;
using Specs.Analyzers;

namespace Resolve_dependencies_specs;

public class Test
{
    [Test]
    public void Resolve()
    {
        var analyzer = new DefaultAnalyzer();

       _ = analyzer.ForCS()
            .AddSource("Snippets/LibraryProject.cs")
            .WithLanguageVersion(LanguageVersion.CSharp11)
            .ReportIssues();

        analyzer.CodeBase.Should().HaveDepedencies(new Dictionary<Symbol, Symbol[]>()
        {
            ["LibraryProject.SomeClass"] = Array.Empty<Symbol>(),
            ["LibraryProject.SomeClass.SomeClass(string)"] = new Symbol[] { "LibraryProject.SomeClass" },
            ["LibraryProject.SomeClass.Name"] = new Symbol[] { "string" },
            ["LibraryProject.SomeClass.BegToDiffer(LibraryProject.OtherClass)"] = new Symbol[] { "LibraryProject.SomeClass", "bool", "LibraryProject.OtherClass" },
            ["LibraryProject.SomeClass.WithContent()"] = new Symbol[] { "System.Console", "bool", "LibraryProject.SomeStruct.SomeStruct()" },

            ["LibraryProject.OtherClass"] = Array.Empty<Symbol>(),

            ["LibraryProject.SomeStruct"] = Array.Empty<Symbol>(),
            ["LibraryProject.SomeStruct.Value"] = new Symbol[] { "LibraryProject.SomeStruct", "int" },
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