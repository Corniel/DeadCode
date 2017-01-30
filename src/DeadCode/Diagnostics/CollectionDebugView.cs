using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeadCode.Diagnostics
{
	/// <summary>Allows the debugger to display collections.</summary>
	public class CollectionDebugView<T>
	{
		/// <summary>Constructor.</summary>
		public CollectionDebugView(IEnumerable<T> enumeration)
		{
			Enumeration = enumeration;
		}

		/// <summary>The array that is shown by the debugger.</summary>
		/// <remarks>
		/// Every time the enumeration is shown in the debugger, a new array is created.
		/// By doing this, it is always in sync with the current state of the enumeration.
		/// </remarks>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items { get { return Enumeration.ToArray(); } }

		/// <summary>A reference to the enumeration to display.</summary>
		private readonly IEnumerable<T> Enumeration;
	}
}
