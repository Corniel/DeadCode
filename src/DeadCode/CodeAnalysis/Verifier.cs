using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Linq;
using System.Threading;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace DeadCode.CodeAnalysis
{
	public static class Verifier
	{
		public  static void Verify(VerifyContext context)
		{
			Guard.NotNull(context, nameof(context));

			using (var workspace = MSBuildWorkspace.Create())
			{
				var solution = workspace.OpenSolutionAsync(context.Solution.FullName).Result;

				foreach (var projectId in solution.ProjectIds)
				{
					using (var cancel = new CancellationTokenSource())
					{
						var project = solution.GetProject(projectId);

						var compilation = project.GetCompilationAsync(cancel.Token).Result
							.WithOptions(project.CompilationOptions)
							.WithAnalyzers(context.Analyzers, cancellationToken: cancel.Token);

						foreach (var diagnostic in compilation.GetAllDiagnosticsAsync().Result.Where(d => d.Id == "AD0001"))
						{
							Console.WriteLine(diagnostic);
						}
					}
				}
			}
		}

		public static CompilationOptions GetCompilationOptions(Compilation compilation)
		{
			switch (compilation.Language)
			{
				case LanguageNames.CSharp:
					return new CS.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
				case LanguageNames.VisualBasic:
					return new VB.VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			}
			throw new NotSupportedException(string.Format("Language '{0}' is not supported.", compilation.Language));
		}
	}
}
