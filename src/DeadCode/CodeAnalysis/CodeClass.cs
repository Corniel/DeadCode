using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class CodeClass: CodePart
	{
		public CodeClass(string key) : base(key)
		{
			Properties = new HashSet<CodeProperty>();
			Methods = new HashSet<CodeMethod>();
		}

		public bool Required { get; set; }

		public static string GetKey(ISymbol symbol)
		{
			Guard.NotNull(symbol, nameof(symbol));
			return $"{symbol.ContainingNamespace}#{symbol.Name}";
		}

		public HashSet<CodeProperty> Properties { get; }
		public HashSet<CodeMethod> Methods { get; }

		public override int Count => Properties.Sum(p => p.Count) + Methods.Sum(m => m.Count);

		public void Add(CodeProperty prop) => Properties.Add(prop);
		public void Add(CodeMethod meth) => Methods.Add(meth);

	}
}
