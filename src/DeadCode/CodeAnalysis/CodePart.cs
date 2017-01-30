using System;

namespace DeadCode.CodeAnalysis
{
	public abstract class CodePart : IComparable<CodePart>, IEquatable<CodePart>
	{
		private readonly int hash;
		protected CodePart(string key)
		{
			Key = Guard.NotNullOrEmpty(key, nameof(key));
			hash = Key.GetHashCode();
		}

		public string Key { get; }

		public abstract int Count { get; }

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
