using CodeAnalysis.TestTools;
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
        

    }
}