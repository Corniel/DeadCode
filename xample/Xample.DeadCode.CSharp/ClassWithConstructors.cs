namespace Xample.DeadCode.CSharp
{
	public class ClassWithConstructors
	{
		public ClassWithConstructors(EmptyClass cls)
		{
			Empty = cls;
		}

		public ClassWithConstructors(EmptyClass cls, int number)
		{
			Empty = number % 2 == 1 ? cls : null;
		}

		public EmptyClass Empty { get; set; }
	}
}
