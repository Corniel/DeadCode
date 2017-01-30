using System;
using System.Collections.Generic;

namespace DeadCode.CodeAnalysis
{
	public abstract class CodeMember : CodePart
	{
		public CodeMember(CodeClass parent, string key) : base(key)
		{
			Parent = Guard.NotNull(parent, nameof(parent));
			CallsTo = new HashSet<CodeMember>();
		}

		public CodeClass Parent { get; }

		public HashSet<CodeMember> CallsTo { get; }

		public override int Count => CallsTo.Count;

		public void AddCallTo(CodeMember call) => CallsTo.Add(call);
	}
}
