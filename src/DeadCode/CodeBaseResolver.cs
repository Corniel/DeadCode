using DeadCode.Syntax;

namespace DeadCode;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CodeBaseResolver : DiagnosticAnalyzer
{
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public CodeBaseResolver() : this(new()) { }

    public CodeBaseResolver(CodeBase codeBase)
    {
        CodeBase = codeBase;
    }

    public CodeBase CodeBase { get; }

    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Visit, SyntaxKind.CompilationUnit);
    }

    private void Visit(SyntaxNodeAnalysisContext context)
    {
        var resolver = new CSharpCodeBaseResolver(CodeBase, context.SemanticModel);
        resolver.Visit(context.Node);
    }

    private static readonly DiagnosticDescriptor Rule = new(
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
