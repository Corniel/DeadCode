using DeadCode;
using DeadCode.Syntax;
using System.Collections.Immutable;

namespace Specs.Tooling;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class CodeBaseAnalyzer : DiagnosticAnalyzer
{
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public CodeBase CodeBase { get; } = new();

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(c =>
        {
            var walker = new CSharpCodeBaseResolver(CodeBase, null!, c.SemanticModel);
            walker.Visit(c.Node);
        },
        SyntaxKind.CompilationUnit);
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
