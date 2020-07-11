using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class Class : CodePart
	{
		public Class(string key) : base(key, key.Substring(key.LastIndexOf('#') + 1))
		{
			Properties = new HashSet<Property>();
			Methods = new HashSet<Method>();
		}

		public override bool IsDefined => m_IsDefined;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_IsDefined;

		public void Define() => m_IsDefined = true;

		public bool Required { get; set; }

		public static string GetKey(ISymbol symbol)
		{
			Guard.NotNull(symbol, nameof(symbol));
			return $"{symbol.ContainingNamespace}#{symbol.Name}";
		}

		public HashSet<Property> Properties { get; }
		public HashSet<Method> Methods { get; }

		public override int Count => Properties.Sum(p => p.Count) + Methods.Sum(m => m.Count);

		public void Add(Property prop) => Properties.Add(prop);
		public void Add(Method meth) => Methods.Add(meth);

	}
}
