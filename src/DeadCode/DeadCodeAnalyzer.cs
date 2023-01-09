using System.IO;

namespace DeadCode.CodeAnalysis;

public abstract class DeadCodeAnalyzer : DiagnosticAnalyzer
{
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public CodeBase CodeBase => DepedencyResolver.CodeBase;
    private DepedencyResolver DepedencyResolver { get; }

    protected DeadCodeAnalyzer() : this(new CodeBase()) { }
    protected DeadCodeAnalyzer(CodeBase codeBase) : this(new DepedencyResolver(codeBase)) { }
    protected DeadCodeAnalyzer(DepedencyResolver? depedencyResolver)
    {
        DepedencyResolver = depedencyResolver ?? new DepedencyResolver(CodeBase);
    }

    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(Analyze);
        //context.RegisterSyntaxNodeAction(Visit, SyntaxKind.CompilationUnit);
    }

    private void Analyze(CompilationAnalysisContext context)
    {
        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            DepedencyResolver.Resolve(tree.GetRoot(), context.Compilation.GetSemanticModel(tree));
        }
        foreach (var code in CodeBase.Code.Where(c => c.Node is { } && !c.Used))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, Location(code.Node!)));
        }
    }
    private static Location Location(SyntaxNode node) => node switch
    {
        ClassDeclarationSyntax x => x.Identifier.GetLocation(),
        StructDeclarationSyntax x => x.Identifier.GetLocation(),
        ConstructorDeclarationSyntax x => x.Identifier.GetLocation(),
        PropertyDeclarationSyntax x => x.Identifier.GetLocation(),
        MethodDeclarationSyntax x => x.Identifier.GetLocation(),
        _ => node.GetLocation(),
    };


    private static void Log(string message)
    {
        lock (locker)
        {
            using var writer = new StreamWriter("c:/TEMP/dead-code.log", true);
            writer.WriteLine(message);
        }
    }
    private static readonly object locker = new();

    //private void Test(SyntaxNodeAnalysisContext context)
    //{
    //    var name = (context.Node as IdentifierNameSyntax)?.Identifier.Text;
    //    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), name));
    //}

    //private void Visit(SyntaxNodeAnalysisContext context)
    //{
    //    DepedencyResolver.Visit(context.Node, context.SemanticModel);

    //    CodeBase.CompilationUnits[context.Node] = true;

    //    Log(CodeBase.CompilationUnits.Count.ToString());

    //    if (CodeBase.Code.All(c => c.Node is { }))
    //    {
    //        Log(string.Join("\n", CodeBase.Symbols));
    //    }
    //}

    protected static readonly DiagnosticDescriptor Rule = new(
       id: "DEAD",
       title: "Dead code should be removed from the solution",
       messageFormat: "Remove this unused code.",
       category: "Maintainability",
       defaultSeverity: DiagnosticSeverity.Warning,
       isEnabledByDefault: true,
       description: "types or members that are never executed or referenced are dead code: unnecessary, inoperative code " +
           "that should be removed. Cleaning out dead code decreases the size of the maintained code base, " +
           "making it easier to understand the program and preventing bugs from being introduced.");
}
