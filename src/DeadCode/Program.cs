using DeadCode.Syntax;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading.Tasks;

namespace DeadCode.App;

static internal class Program
{
    static async Task Main(string[] args)
    {
        var codeBase = new CodeBase();
        var analyzers = (new DiagnosticAnalyzer[] { new CodeBaseResolver(codeBase) }).ToImmutableArray();

        var instance = MSBuildLocator.QueryVisualStudioInstances()
            .OrderByDescending(i => i.Version)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("No instance of Visual Studio detected.");

        MSBuildLocator.RegisterInstance(instance);
        
        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (o, e) => Console.Error.WriteLine(e.Diagnostic.Message);
        
        var solution = await workspace.OpenSolutionAsync(@"C:\_TJIP\sima-pro\SimaProPlatform.sln", new ConsoleProgressReporter());

        var sw = Stopwatch.StartNew();

        foreach (var project in solution.Projects.Where(ExcludeTests))
        {
            var compilation = (await project.GetCompilationAsync())!.WithAnalyzers(analyzers);
            await compilation.GetAllDiagnosticsAsync();
        }
        sw.Stop();

        Console.WriteLine($"nodes: {codeBase.Code.Count} ({sw.ElapsedMilliseconds} ms)");
        Console.WriteLine($"used: {codeBase.Code.Count(c => !c.IsDead)}");

        var test = codeBase.Code.Where(c => !c.IsDead).ToArray();

        Console.ReadKey();
    }

    private static bool ExcludeTests(Project project)
    {
        return !project.AssemblyName.Contains("Test")
            && !project.AssemblyName.Contains(".Specs")
            && !project.AssemblyName.Contains("Benchmarks");
    }
}
