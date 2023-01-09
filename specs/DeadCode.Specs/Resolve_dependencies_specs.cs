using CodeAnalysis.TestTools;
using DeadCode;
using FluentAssertions;
using Specs;
using Specs.Analyzers;

namespace Resolve_dependencies_specs;

public class Test
{
    [Test]
    public void ctor_depends_on_base()
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

        analyzer.CodeBase.Should().HaveDepedencies(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Refs("MyClass.MyClass()"),
            ["MyClass.MyClass()"] = Symbol.Refs("MyClass"),
            ["MyClass.MyClass(MyClass)"] = Symbol.Refs("MyClass", "MyClass.MyClass()"),
        });
    }

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
            ["LibraryProject.SomeClass"] = Symbol.Refs(),
            ["LibraryProject.SomeClass.SomeClass(string)"] = Symbol.Refs("LibraryProject.SomeClass"),
            ["LibraryProject.SomeClass.Name"] = Symbol.Refs(),
            ["LibraryProject.SomeClass.BegToDiffer(LibraryProject.OtherClass)"] = Symbol.Refs("LibraryProject.SomeClass", "bool", "LibraryProject.OtherClass"),
            ["LibraryProject.SomeClass.WithContent()"] = Symbol.Refs("System.Console", "bool", "LibraryProject.SomeStruct.SomeStruct()"),

            ["LibraryProject.OtherClass"] = Symbol.Refs(),

            ["LibraryProject.SomeStruct"] = Symbol.Refs(),
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