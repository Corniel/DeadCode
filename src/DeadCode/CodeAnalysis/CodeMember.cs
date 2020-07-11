using System;
using System.Collections.Generic;

namespace DeadCode.CodeAnalysis
{
	public abstract class CodeMember : CodePart
	{
		public CodeMember(Class parent, string key) : base(key, key.Substring(key.LastIndexOf(':') + 1))
		{
			Parent = Guard.NotNull(parent, nameof(parent));
		}

		public Class Parent { get; }

		public override bool IsDefined => Parent.IsDefined;

		public override int Count => CallsTo.Count;

		public void AddCallTo(CodeMember call) => CallsTo.Add(call);
	}
}
