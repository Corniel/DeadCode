using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class VerifyContext
	{
		public VerifyContext(FileInfo solution, CodeParts parts)
		{
			Solution = Guard.Exists(solution, nameof(solution));
			CSAnalyzer = new DeadCodeAnalyzer(new DeadCodeCSharpWalker(parts));
			VBAnalyzer = new DeadCodeAnalyzer(new DeadCodeVisualBasicWalker(parts));
			Analyzers = new[] { CSAnalyzer, VBAnalyzer }.ToImmutableArray<DiagnosticAnalyzer>();
			Id = CSAnalyzer.SupportedDiagnostics.Single().Id;
		}
		public FileInfo Solution { get; }

		public DeadCodeAnalyzer CSAnalyzer { get; }
		public DeadCodeAnalyzer VBAnalyzer { get; }
		public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

		public string Id { get; }

		public Dictionary<string, ReportDiagnostic> ReportDiagnostics
		{
			get
			{
				var diagnostics = CSAnalyzer.SupportedDiagnostics
					.ToDictionary(d => d.Id, d => ReportDiagnostic.Warn);
				diagnostics["AD0001"] = ReportDiagnostic.Error;
				return diagnostics;
			}
		}
	}
}
