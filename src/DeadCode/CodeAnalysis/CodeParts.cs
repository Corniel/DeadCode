using DeadCode.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	[DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(CollectionDebugView<CodePart>))]
	public class CodeParts : IEnumerable<CodePart>
	{
		private readonly Dictionary<string, CodePart> collection = new Dictionary<string, CodePart>();

		public CodePart this[string key] => collection[key];

		public int Count => collection.Count;

		public IEnumerable<CodeClass> Classes => this.Where(p => p is CodeClass).Cast<CodeClass>();

		public CodeClass TryGetClass(INamedTypeSymbol symbol)
		{
			var key = CodeClass.GetKey(symbol);
			CodePart cls;
			collection.TryGetValue(key, out cls);
			return (CodeClass)cls;
		}

		public CodeClass GetClass(ISymbol symbol)
		{
			var key = CodeClass.GetKey(symbol);

			CodePart cls;
			if (!collection.TryGetValue(key, out cls))
			{
				cls = new CodeClass(key);
				collection[key] = cls;
			}
			return (CodeClass)cls;
		}

		public CodeProperty GetProperty(CodeClass cls, IPropertySymbol symbol)
		{
			string key = CodeProperty.GetKey(cls, symbol);
			CodePart prop;
			if (!collection.TryGetValue(key, out prop))
			{
				prop = new CodeProperty(cls, key);
				collection[key] = prop;
				cls.Add((CodeProperty)prop);
			}
			return (CodeProperty)prop;
		}

		public CodeMethod GetMethod(CodeClass cls, IMethodSymbol symbol)
		{
			string key = CodeMethod.GetKey(cls, symbol);
			CodePart meth;
			if (!collection.TryGetValue(key, out meth))
			{
				meth = new CodeMethod(cls, key);
				collection[key] = meth;
				cls.Add((CodeMethod)meth);
			}
			return (CodeMethod)meth;
		}

		public int RemovePart(CodePart part)
		{
			Guard.NotNull(part, nameof(part));
			var sum = 0;

			CodePart selected;
			if(collection.TryGetValue(part.Key, out selected))
			{
				
				foreach(var called in selected.CallsTo)
				{
					sum += RemovePart(part);
				}
				collection.Remove(part.Key);
			}
			return sum;
		}

		public IEnumerator<CodePart> GetEnumerator() => collection.Values.OrderBy(v => v).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
