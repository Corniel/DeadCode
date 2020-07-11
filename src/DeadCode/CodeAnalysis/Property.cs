using Microsoft.CodeAnalysis;

namespace DeadCode.CodeAnalysis
{
	public class Property : CodeMember
	{
		public Property(Class parent, string key) : base(parent, key) { }

		public static string GetKey(Class cls, IPropertySymbol symbol)
		{
			Guard.NotNull(cls, nameof(cls));
			Guard.NotNull(symbol, nameof(symbol));
			return $"{cls.Key}:{symbol.Name}";
		}
	}
}
