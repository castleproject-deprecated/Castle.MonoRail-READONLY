namespace Castle.DynamicProxy.Tests.GenClasses
{
	public class GenClassNameClash<T, Z>
	{
		public virtual void DoSomethingT<T>(T t)
		{
		}

		public virtual void DoSomethingZ<Z>(Z z)
		{
		}

		public virtual void DoSomethingTX<T, X>(T t, X x)
		{
		}

		public virtual void DoSomethingZX<Z, X>(Z z, X x)
		{
		}
	}
}
