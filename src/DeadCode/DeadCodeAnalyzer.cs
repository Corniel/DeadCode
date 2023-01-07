using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace DeadCode.CodeAnalysis;

public abstract class DeadCodeAnalyzer : DiagnosticAnalyzer
{
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    private readonly CodeBase CodeBase = new();

    public CodeWalker Walker { get; }

    protected DeadCodeAnalyzer()
    {
        Walker = new CodeWalker(CodeBase);
    }

    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(Register);
        context.RegisterSyntaxNodeAction(Test, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(Visit, SyntaxKind.CompilationUnit);
    }

    private void Register(CompilationAnalysisContext context)
    {
        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            if (tree.GetRoot().IsKind(SyntaxKind.CompilationUnit))
            {

                CodeBase.CompilationUnits[tree.GetRoot()] = false;
            }
        }
    }

    private static void Log(string message)
    {
        lock (locker)
        {
            using var writer = new StreamWriter("c:/TEMP/dead-code.log", true);
            writer.WriteLine(message);
        }
    }
    private static readonly object locker = new();

    private void Test(SyntaxNodeAnalysisContext context)
    {
        var name = (context.Node as IdentifierNameSyntax)?.Identifier.Text;
        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), name));
    }

    private void Visit(SyntaxNodeAnalysisContext context)
    {
        Walker.Visit(context.Node, context.SemanticModel);

        CodeBase.CompilationUnits[context.Node] = true;

        Log(CodeBase.CompilationUnits.Count.ToString());

        if (CodeBase.Code.All(c => c.Identifier.HasValue))
        {
            Log(string.Join("\n", CodeBase.Symbols));
        }
    }

    protected static readonly DiagnosticDescriptor Rule = new(
       id: "DEAD",
       title: "Dead code should be removed from the solution",
       messageFormat: @"Code '{0}' is not used.",
       category: "Maintainability",
       defaultSeverity: DiagnosticSeverity.Warning,
       isEnabledByDefault: true,
       description: "types or members that are never executed or referenced are dead code: unnecessary, inoperative code " +
           "that should be removed. Cleaning out dead code decreases the size of the maintained code base, " +
           "making it easier to understand the program and preventing bugs from being introduced.");
}
