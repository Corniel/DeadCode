using System;
using System.Collections.Generic;

namespace DeadCode.CodeAnalysis
{
	public abstract class CodePart : IComparable<CodePart>, IEquatable<CodePart>
	{
		private readonly int hash;
		protected CodePart(string key, string name)
		{
			Key = Guard.NotNullOrEmpty(key, nameof(key));
			hash = Key.GetHashCode();
			CallsTo = new HashSet<CodeMember>();
			Name = name;
		}

		public string Key { get; }

		public abstract int Count { get; }

		public string Name { get; }

		public abstract bool IsDefined { get; }

		public HashSet<CodeMember> CallsTo { get; }

		public override int GetHashCode() => hash;
		public override bool Equals(object obj) => Equals(obj as CodePart);

		public bool Equals(CodePart other)
		{
			return
				other != null &&
				hash == other.hash &&
				Key == other.Key;
		}

		public int CompareTo(CodePart other) => Key.CompareTo(other.Key);
		public override string ToString() => Key;
	}
}
