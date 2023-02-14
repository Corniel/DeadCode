using CommandLine;
using DeadCode.Application;
using DeadCode.Editing;
using DeadCode.Syntax;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System.IO;
using System.Threading.Tasks;

namespace DeadCode;

public static class DeadCodeAnalyzer
{
    public static Task Run(params string[] args)
        => Parser.Default.ParseArguments<DeadCodeAnalyzerOptions>(args).WithParsedAsync(Run);

    public static async Task Run(DeadCodeAnalyzerOptions options)
    {
        Guard.NotNull(options, nameof(options));

        MSBuildLocator.RegisterDefaults();
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
            code.IsEntryPoint = DepedencyResolver.IsEntryPoint(code.Symbol);
        }

        Console.WriteLine($"used: {codeBase.Code.Count(c => !c.IsDead)}");

        DeadCodeLogger.Writer = new StreamWriter("c:/TEMP/dead-code.log", false);
        await DeadCodeLogger.Apply(codeBase);

        //await DeadCodeDecorator.Change(codeBase);
        //await DeadCodeRemover.Change(codeBase);
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
