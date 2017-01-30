using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace DeadCode.CodeAnalysis
{
	[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
	public class DeadCodeAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "DEADCODE";
		internal const string Title = "Dead code should be removed from the solution";
		internal const string Description =
			"types or members that are never executed or referenced are dead code: unnecessary, inoperative code " +
			"that should be removed. Cleaning out dead code decreases the size of the maintained code base, " +
			"making it easier to understand the program and preventing bugs from being introduced.";

		internal const string MessageFormat = "Code \"{0}\" is not used.";
		internal const string Category = "Dead Code";
		internal const DiagnosticSeverity RuleSeverity = DiagnosticSeverity.Warning;
		internal const bool IsActivatedByDefault = true;

		internal static readonly DiagnosticDescriptor Rule =
			new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
				RuleSeverity, IsActivatedByDefault,
				description: Description);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public DeadCodeAnalyzer(DeadCodeWalker walker)
		{
			Walker = Guard.NotNull(walker, nameof(walker));
		}

		public static readonly CS.SyntaxKind[] CSharpKinds = new[]
		{
			CS.SyntaxKind.ClassDeclaration
		};
		public static readonly VB.SyntaxKind[] VisualBasicKinds = new[]
		{
			VB.SyntaxKind.ClassBlock
		};

		public DeadCodeWalker Walker { get; }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(ScanCode, CSharpKinds);
			context.RegisterSyntaxNodeAction(ScanCode, VisualBasicKinds);
		}

		private void ScanCode(SyntaxNodeAnalysisContext context)
		{
			Walker.Visit(context.Node, context.SemanticModel);
		}
	}
}
