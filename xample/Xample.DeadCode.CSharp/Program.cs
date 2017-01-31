using System;

namespace Xample.DeadCode.CSharp
{
	static class Program
	{
		static void Main(params object[] args)
		{
			var x = new SimpleClass();
			Console.Write(x.Method(8));
		}
	}
}
