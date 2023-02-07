using DeadCode;
using DeadCode.Syntax;
using System.Collections.Immutable;

namespace Specs.Tooling;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class CollectionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => default;

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
}
