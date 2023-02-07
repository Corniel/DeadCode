using CommandLine;
using DeadCode.App;
using DeadCode.Application;
using DeadCode.Editing;
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
        
        var instance = MSBuildLocator.QueryVisualStudioInstances()
            .OrderByDescending(i => i.Version)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("No instance of Visual Studio detected.");

        MSBuildLocator.RegisterInstance(instance);

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (o, e) => Console.Error.WriteLine(e.Diagnostic.Message);

        var solution = await workspace.OpenSolutionAsync(options.Solution, new ConsoleProgressReporter());

        var sw = Stopwatch.StartNew();

        var project = solution.Projects.Where(ExcludeTests);
        var codeBase = await Collect(project);

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

        DeadCodeRemover.Change(codeBase);

    }

    public static async Task<CodeBase> Collect(IEnumerable<Project> projects)
    {
        Guard.NotNull(projects, nameof(projects));

        var codebase = new CodeBase();

        foreach (var project in projects)
        {
            foreach(var document in project.Documents)
            {
                if (await document.GetSyntaxRootAsync() is { } root
                    && await document.GetSemanticModelAsync() is { } model)
                {
                    var resolver = new CSharpCodeBaseResolver(codebase, document, model);
                    resolver.Visit(root);
                }
            }
        }
        return codebase;
    }

    private static bool ExcludeTests(Project project)
    {
        return !project.AssemblyName.Contains("Test")
            && !project.AssemblyName.Contains(".Specs")
            && !project.AssemblyName.Contains("Benchmarks");
    }
}
