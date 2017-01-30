using DeadCode.CodeAnalysis;
using System;
using System.Configuration;
using System.IO;

namespace DeadCode
{
	public static class Program
	{
		public static void Main() => Main(ConfigurationManager.AppSettings["SolutionPath"]);
		public static void Main(string path) => Analyse(new FileInfo(path));

		public static void Analyse(FileInfo solution)
		{
			var parts = new CodeParts();
			var context = new VerifyContext(solution, parts);
			Verifier.Verify(context);

			foreach (var part in parts)
			{
				Console.WriteLine($"{part}, {part.Count}");
			}
		}
	}
}
