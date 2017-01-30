namespace Xample.DeadCode.CSharp
{
	public class SimpleClass
	{
		public int Prop { get; set; }

		public int Twice { get { return Prop * 2; } }
		public int Method(int val)
		{
			return val + Prop;
		}
	}
}
