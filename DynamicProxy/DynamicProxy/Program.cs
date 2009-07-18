using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;

namespace DynamicProxy
{
    public class Program
    {
        public interface IFoo
        {
            void FooMethod(out int i);
            int BarMethod(int i);
        }

        public class Foo : IFoo
        {
            void IFoo.FooMethod(out int i)
            {
                Console.WriteLine("From Foo.FooMethod!");
                i = 100;
            }

            int IFoo.BarMethod(int i)
            {
                return i;
            }
        }

        public static void Main(string[] args)
        {
            const string moduleName = "DynamicModule";
            ModuleBuilder mb = EmitUtils.CreateDynamicModule(moduleName);

            Type t = OverrideFoo(mb);

            EmitUtils.SaveAssembly(moduleName);

            var proxy = (IFoo) t.GetConstructor(Type.EmptyTypes).Invoke(null);

            int result;
            proxy.FooMethod(out result);
            Console.WriteLine(result);
            Console.WriteLine(proxy.BarMethod(5));
#if !DEBUG
            Console.WriteLine("Warm up:");
            PerformanceTest(proxy, new Foo(), 1000);
            Console.WriteLine("Test:");
            PerformanceTest(proxy, new Foo(), 20000);
#endif
        }

        private static void PerformanceTest(IFoo proxy, IFoo original, int loop1)
        {
            Stopwatch stopwatch = new Stopwatch();
            int result = 0;
            stopwatch.Start();
            for (int i = loop1 - 1; i >= 0; i--)
            {
                for (int j = loop1 - 1; j >= 0; j--)
                {
                    result += original.BarMethod(1);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Original result " + result + " took " + stopwatch.ElapsedTicks);
            stopwatch.Reset();

            result = 0;
            stopwatch.Start();
            for (int i = loop1 - 1; i >= 0; i--)
            {
                for (int j = loop1 - 1; j >= 0; j--)
                {
                    result += proxy.BarMethod(1);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Proxy result " + result + " took " + stopwatch.ElapsedTicks);
            stopwatch.Reset();
        }

        private static Type OverrideFoo(ModuleBuilder mb)
        {
            TypeBuilder tb = mb.DefineType(
                mb.Assembly.GetName().Name + ".Bar", TypeAttributes.Public | TypeAttributes.Class, 
                typeof(Foo), new Type[]{typeof(IFoo)});
            var iFooMethods = typeof (IFoo).GetMethods();
            var overriders = new List<ExplicitMethodOverrider>(iFooMethods.Length);
            foreach (MethodInfo iFooMethod in iFooMethods)
            {
                overriders.Add(new ExplicitMethodOverrider(mb, tb, iFooMethod));
            }

            Type t = tb.CreateType();
            // Initialize static fields for delegates
            foreach (ExplicitMethodOverrider overrider in overriders)
            {
                overrider.InitializeDelegate(t);
            }
            return t;
        }
    }
}
