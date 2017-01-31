using System;
using System.Collections.Generic;

namespace DeadCode.CodeAnalysis
{
	public abstract class CodeMember : CodePart
	{
		public CodeMember(CodeClass parent, string key) : base(key)
		{
			Parent = Guard.NotNull(parent, nameof(parent));
		}

		public CodeClass Parent { get; }

		public override bool IsDefined => Parent.IsDefined;

		public override int Count => CallsTo.Count;

		public void AddCallTo(CodeMember call) => CallsTo.Add(call);
	}
}
