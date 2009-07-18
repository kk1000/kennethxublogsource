using System;

namespace DynamicProxy
{
    /// <summary>
    /// This is used to create IL sample for proxy implementation
    /// </summary>
    public class Bar : Program.Foo, Program.IFoo
    {
        private delegate void HasOut(Program.Foo foo, out int i);

        private static HasOut Base_1_FooMethod;
        private static Func<Program.Foo, int, int> Base_2_Bar_Method;

        public int BarMethod(int i)
        {
            Console.WriteLine("Proxy before call Int32 DynamicProxy.Program.IFoo.BarMethod(Int32)");
            int j = Base_2_Bar_Method(this, i);
            Console.WriteLine("Proxy after call Int32 DynamicProxy.Program.IFoo.BarMethod(Int32)");
            return j;
        }

        public void FooMethod(out int i)
        {
            Console.WriteLine("Proxy before call Void DynamicProxy.Program.IFoo.FooMethod(Int32 ByRef)");
            Base_1_FooMethod(this, out i);
            Console.WriteLine("Proxy after call Void DynamicProxy.Program.IFoo.FooMethod(Int32 ByRef)");
        }
    }
}
