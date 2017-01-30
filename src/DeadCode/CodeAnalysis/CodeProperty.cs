using Microsoft.CodeAnalysis;

namespace DeadCode.CodeAnalysis
{
	public class CodeProperty : CodeMember
	{
		public CodeProperty(CodeClass parent, string key) : base(parent, key) { }

		public static string GetKey(CodeClass cls, IPropertySymbol symbol)
		{
			Guard.NotNull(cls, nameof(cls));
			Guard.NotNull(symbol, nameof(symbol));
			return $"{cls.Key}:{symbol.Name}";
		}
	}
}
