using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.IO;

namespace DeadCode.CodeAnalysis;

public abstract class DeadCodeAnalyzer : DiagnosticAnalyzer
{
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public CodeParts Parts { get; }

    public DeadCodeWalker Walker { get; }

    protected DeadCodeAnalyzer()
    {
        Parts = new();
        Walker = new DeadCodeCSharpWalker(Parts);
    }

    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(Visit, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(Test, SyntaxKind.IdentifierName);
    }

    private static void Log(string message)
    {
        using var writer = new StreamWriter("c:/TEMP/dead-code.log", new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate,
            Share = FileShare.Write
        });
        writer.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: {message}");
    }
private void Test(SyntaxNodeAnalysisContext context)
    {
        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), (context.Node as IdentifierNameSyntax)?.Identifier.Text));
    }

    private void Visit(SyntaxNodeAnalysisContext context)
    {
        //Walker.Visit(context.Node);
        Log($"Parts: {Parts?.Count}");
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
