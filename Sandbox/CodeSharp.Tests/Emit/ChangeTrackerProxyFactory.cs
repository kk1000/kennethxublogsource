using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CodeSharp.Emit
{
    public class ChangeTrackerProxyFactory
    {
        const string _moduleName = "ChangeTrackerProxyFactory.dll";
        const string _namespace = _moduleName;
        private static readonly Predicate<Type> _noDeepProxy = t => false;

        private static AssemblyBuilder _assemblyBuilder;
        private static ModuleBuilder _moduleBuilder;
        private static readonly MethodInfo _firePropertyChanged;
        private static Predicate<Type> _deepProxyFilter = _noDeepProxy;

        static ChangeTrackerProxyFactory()
        {
            AssemblyName an = new AssemblyName { Name = _moduleName };
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            _assemblyBuilder = ab;
            _moduleBuilder = ab.DefineDynamicModule(_moduleName, _moduleName);
            _firePropertyChanged = typeof (ChangeTrackerBase).GetMethod(
                "FirePropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic, 
                null, new[]{typeof (string)}, null);
        }

        internal static void SaveAssembly()
        {
            _assemblyBuilder.Save(_moduleName);
        }

        public static Predicate<Type> DeepProxyFilter
        {
            set
            {
                _deepProxyFilter = value ?? _noDeepProxy;
            }
        }

        public static void SetDeepProxyAttribute<TA>()
            where TA : Attribute
        {
            _deepProxyFilter = t => t.GetCustomAttributes(typeof (TA), true).Length != 0;
        }

        public static T NewProxy<T>(T target)
        {
            return Generator<T>.NewProxy(target);
        }

        struct Generator<T>
        {
            static readonly Converter<T, T> _wrap;
            static readonly Type _type;

            private IGenerator g;
            private IField wrapped;
            private IClass wrapper;


            static Generator()
            {
                var @interface = typeof(T);
                if (!@interface.IsInterface)
                    throw new InvalidOperationException(@interface + " is not an interface type.");
                Emitter e = new Emitter(_moduleBuilder);
                _type = e.Generate(new Generator<T>{g=e}.DefineWrapper());
                //var factoryMethod = _type.GetMethod("CreateProxy", new[] {@interface});
                //_wrap = (Converter<T, T>) Delegate.CreateDelegate(typeof(Converter<T, T>), factoryMethod);

                var constructor = _type.GetConstructor(new[] { @interface });
                _wrap = x => (T)constructor.Invoke(new object[] { x });
            }

            public static T NewProxy(T target)
            {
                return _wrap(target);
            }

            private IClass DefineWrapper()
            {
                Type @interface = typeof (T);
                wrapper = g.Class("ChangeCheckerFor" + @interface.Name).In(_namespace).Inherits<ChangeTrackerBase>().Implements(@interface);
                {
                    wrapped = wrapper.Field(@interface, "_wrapped").Internal.ReadOnly;

                    var ctor = wrapper.Constructor(g.Arg(@interface, "wrapped")).Public;
                    using (var c1 = ctor.Code())
                    {
                        c1.Assign(wrapped, ctor.Args[0]);
                    }

                    foreach (MemberInfo info in @interface.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    {
                        switch (info.MemberType)
                        {
                            case MemberTypes.Method:
                                MethodInfo mi = (MethodInfo)info;
                                if (!mi.IsSpecialName)
                                {
                                    WrapMethod(mi);
                                }
                                break;
                            case MemberTypes.Property:
                                PropertyInfo pi = (PropertyInfo)info;
                                if (_deepProxyFilter(pi.PropertyType))
                                    WrapDeepProxiedProperty(pi);
                                else
                                    WrapProperty(pi);
                                break;
                            default:
                                Console.WriteLine(info + " : " + info.MemberType);
                                break;
                        }
                    }
                }
                return wrapper;
            }

            private void WrapMethod(MethodInfo mi)
            {
                var method = wrapper.Method(mi).Public;
                using (var code = method.Code())
                {
                    code.Return(wrapped.Invoke(mi, method.Args.AsOperands()));
                }
            }

            private void WrapProperty(PropertyInfo pi)
            {
                var property = wrapper.Property(pi).Public;
                if (pi.CanRead)
                {
                    var getter = property.Getter();
                    using (var c = getter.Code())
                    {
                        c.Return(wrapped.Property(pi, getter.Args.AsOperands()));
                    }
                }
                if (pi.CanWrite)
                {
                    var setter = property.Setter();
                    using (var c = setter.Code())
                    {
                        //TODO: we are doing indexers too, should we?
                        c.If(c.AreNotEqual(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value));
                        {
                            c.Assign(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value);
                            c.Call(wrapper.This.Invoke(_firePropertyChanged, g.Const(pi.Name)));
                        }
                        c.End();
                    }
                }
            }

            private void WrapDeepProxiedProperty(PropertyInfo pi)
            {
                var generatorType = typeof(Generator<T>).GetGenericTypeDefinition().MakeGenericType(pi.PropertyType);
                var proxyType = (Type) generatorType.GetField("_type", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var backingField = wrapper.Field(proxyType, "_" + pi.Name);
                var constructor = proxyType.GetConstructor(new[] { proxyType });
                Console.WriteLine(constructor);
                var property = wrapper.Property(pi).Public;
                if (pi.CanRead)
                {
                    var getter = property.Getter();
                    using (var c = getter.Code())
                    {
                        // var p = _wrapped.ComponentProperty;
                        var p = c.Variable(pi.PropertyType, "p");
                        c.Assign(p, wrapped.Property(pi, getter.Args.AsOperands()));

                        // if (p == null)
                        c.If(c.IsNull(p));
                        {
                            var @null = g.Null(pi.PropertyType);
                            // _ComponentProperty = null;
                            c.Assign(backingField, @null);
                            // reurn null;
                            c.Return(@null);
                        }
                        c.End();

                        // if (_ComponentProperty == null || !ReferenceEquals(_ComponentProperty._wrapped, p))
                        c.If(c.Or(c.IsNull(backingField), c.NotReferenceEquals(p, backingField.Field("_wrapped"))));
                        {
                            // _ComponentProperty = new ChangeTrackerForIValueComponent(p);
                            c.Assign(backingField, c.New(proxyType, p));
                        }
                        c.End();

                        //return _ComponentProperty;
                        c.Return(backingField);
                    }
                }
                if (pi.CanWrite)
                {
                    var setter = property.Setter();
                    using (var c = setter.Code())
                    {
                        //TODO: we are doing indexers too, should we?
                        c.If(c.AreNotEqual(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value));
                        {
                            c.Assign(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value);
                            c.Call(wrapper.This.Invoke(_firePropertyChanged, g.Const(pi.Name)));
                        }
                        c.End();
                    }
                }
            }
        }

    }
}
