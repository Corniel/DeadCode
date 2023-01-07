using DeadCode.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	[DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(CollectionDebugView<CodePart>))]
	public class CodeParts : IReadOnlyCollection<CodePart>
	{
		private readonly Dictionary<string, CodePart> collection = new Dictionary<string, CodePart>();

		public CodePart this[string key] => collection[key];

		public int Count => collection.Count;

		public IEnumerable<Class> DefinedClasses => this.Where(p => p.IsDefined && p is Class).Cast<Class>();

		public IEnumerable<CodeMember> DefinedMembers => this.Where(p => p.IsDefined && p is CodeMember).Cast<CodeMember>();

		public IEnumerable<Method> DefinedMethods => this.Where(p => p.IsDefined && p is Method).Cast<Method>();

		public Class GetClass(ISymbol symbol)
		{
			var key = Class.GetKey(symbol);

			CodePart cls;
			if (!collection.TryGetValue(key, out cls))
			{
				cls = new Class(key);
				collection[key] = cls;
			}
			return (Class)cls;
		}

		public Property GetProperty(Class cls, IPropertySymbol symbol)
		{
			string key = Property.GetKey(cls, symbol);
			CodePart prop;
			if (!collection.TryGetValue(key, out prop))
			{
				prop = new Property(cls, key);
				collection[key] = prop;
				cls.Add((Property)prop);
			}
			return (Property)prop;
		}

        public Property GetProperty(IPropertySymbol symbol)
        {
            var cls = GetClass(symbol.ContainingType);
            string key = Property.GetKey(cls, symbol);

            if (!collection.TryGetValue(key, out var prop))
            {
                prop = new Property(cls, key);
                collection[key] = prop;
                cls.Add((Property)prop);
            }
            return (Property)prop;
        }

        public Method GetMethod(Class cls, IMethodSymbol symbol)
		{
			string key = Method.GetKey(cls, symbol);
			CodePart meth;
			if (!collection.TryGetValue(key, out meth))
			{
				meth = new Method(cls, key);
				collection[key] = meth;
				cls.Add((Method)meth);
			}
			return (Method)meth;
		}

        public Method GetMethod(IMethodSymbol symbol)
        {
			var cls = GetClass(symbol.ContainingType);
            string key = Method.GetKey(cls, symbol);

			if (!collection.TryGetValue(key, out var meth))
            {
                meth = new Method(cls, key);
                collection[key] = meth;
                cls.Add((Method)meth);
            }
            return (Method)meth;
        }



        public int RemovePart(CodePart part)
		{
			Guard.NotNull(part, nameof(part));
			var sum = 0;

			CodePart selected;
			if(collection.TryGetValue(part.Key, out selected))
			{
				collection.Remove(part.Key);
				foreach (var called in selected.CallsTo)
				{
					sum += RemovePart(part);
				}
			}
			return sum;
		}

		public IEnumerator<CodePart> GetEnumerator() => collection.Values.OrderBy(v => v).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
