using Microsoft.CodeAnalysis;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class CodeConstructor : CodeMember
	{
		public CodeConstructor(CodeClass parent, string key) : base(parent, key) { }

		public static string GetKey(CodeClass cls, IMethodSymbol symbol)
		{
			Guard.NotNull(cls, nameof(cls));
			Guard.NotNull(symbol, nameof(symbol));
			var ps = string.Join(",", symbol.Parameters.Select(p => $"{p.Type.ContainingNamespace}#{p.Type.Name}"));
			return $"{cls.Key}::Ctor({ps})";
		}
	}
}
