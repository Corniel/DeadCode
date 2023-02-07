using CommandLine;
using DeadCode.App;
using DeadCode.Application;
using DeadCode.Syntax;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading.Tasks;

namespace DeadCode;

public static class DeadCodeAnalyzer
{
    public static Task Run(params string[] args)
        => Parser.Default.ParseArguments<DeadCodeAnalyzerOptions>(args).WithParsedAsync(Run);

    public static async Task Run(DeadCodeAnalyzerOptions options)
    {
        Guard.NotNull(options, nameof(options));

        var codeBase = new CodeBase();
        var analyzers = (new DiagnosticAnalyzer[] { new CodeBaseResolver(codeBase) }).ToImmutableArray();

        var instance = MSBuildLocator.QueryVisualStudioInstances()
            .OrderByDescending(i => i.Version)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("No instance of Visual Studio detected.");

        MSBuildLocator.RegisterInstance(instance);

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (o, e) => Console.Error.WriteLine(e.Diagnostic.Message);

        var solution = await workspace.OpenSolutionAsync(options.Solution, new ConsoleProgressReporter());

        var sw = Stopwatch.StartNew();

        var documents = new Dictionary<string, Document>();

        foreach (var project in solution.Projects.Where(ExcludeTests))
        {
            var compilation = (await project.GetCompilationAsync())!.WithAnalyzers(analyzers);
            await compilation.GetAllDiagnosticsAsync();

            foreach(var doc in project.Documents.Where(d => d.FilePath is { }))
            {
                documents[doc.FilePath!] = doc;
            }
        }
        sw.Stop();

        Console.WriteLine($"nodes: {codeBase.Code.Count} ({sw.ElapsedMilliseconds} ms)");

        foreach (var code in codeBase.Code)
        {
            if(code.Symbol is IMethodSymbol method)
            {
                code.IsEntryPoint = DepedencyResolver.IsEntryPoint(method);
            }
        }

        Console.WriteLine($"used: {codeBase.Code.Count(c => !c.IsDead)}");

        


        var alive = codeBase.Code.Where(c => !c.IsDead).ToArray();
        var dead = codeBase.Code.Where(c => c.IsDead).ToArray();
    }

    private static bool ExcludeTests(Project project)
    {
        return !project.AssemblyName.Contains("Test")
            && !project.AssemblyName.Contains(".Specs")
            && !project.AssemblyName.Contains("Benchmarks");
    }
}
