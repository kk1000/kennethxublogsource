using System;
using System.Collections.Generic;
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
                return i*i;
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
