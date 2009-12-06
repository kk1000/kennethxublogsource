using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CodeSharp.Emit
{
    public static class NotifyPropertyChangedProxyFactory
    {
        const string _moduleName = "NotifyPropertyChangedProxyFactory.dll";
        const string _namespace = _moduleName;
        private const string _defaultOnPropertyChangedMethod = "OnPropertyChanged";
        private static readonly Predicate<Type> _noDeepProxy = t => false;

        private static AssemblyBuilder _assemblyBuilder;
        private static ModuleBuilder _moduleBuilder;
        private static MethodInfo _onPropertyChanged;
        private static Type _baseClassType;
        private static Predicate<Type> _deepProxyFilter = _noDeepProxy;
        private static List<IWeakCollection> _caches = new List<IWeakCollection>();
        private static int _isCleanupInProcess;
        private static readonly Timer _cacheCleanupTimer = new Timer(CleanupCaches, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        private static object _initLock = new object();
        private static bool _isInitLocked = false;

        static NotifyPropertyChangedProxyFactory()
        {
            AssemblyName an = new AssemblyName { Name = _moduleName };
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            _assemblyBuilder = ab;
            _moduleBuilder = ab.DefineDynamicModule(_moduleName, _moduleName);
            SetBaseClass<NotifyPropertyChangedBase>();
        }

        public static void SetBaseClass<TBase>()
        {
            SetBaseClass<TBase>(_defaultOnPropertyChangedMethod);
        }

        public static void SetBaseClass(Type baseClassType)
        {
            SetBaseClass(baseClassType, _defaultOnPropertyChangedMethod);
        }

        public static void SetBaseClass<TBase>(string onPropertyChangeMethod)
        {
            SetBaseClass(typeof(TBase), onPropertyChangeMethod);
        }

        public static void SetBaseClass(Type baseClassType, string onPropertyChangeMethod)
        {
            if (baseClassType == null) throw new ArgumentNullException("baseClassType");
            if (onPropertyChangeMethod == null) throw new ArgumentNullException("onPropertyChangeMethod");
            lock(_initLock)
            {
                RequiresNotInitLocked();
                var onPropertyChanged = baseClassType.GetMethod(
                    onPropertyChangeMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new[] { typeof(string) }, null);
                if (onPropertyChanged == null || !(onPropertyChanged.IsPublic || onPropertyChanged.IsFamily) || onPropertyChanged.IsAbstract)
                {
                    throw new ArgumentException(string.Format(
                        "Public or protected method {0}.{1}(string) doesn't exist.", 
                        baseClassType.FullName, onPropertyChangeMethod));
                }
                _baseClassType = baseClassType;
                _onPropertyChanged = onPropertyChanged;
            }
        }

        private static void RequiresNotInitLocked()
        {
            if (_isInitLocked)
            {
                throw new InvalidOperationException(
                    "This method can only be called before any proxy object are created.");
            }
        }

        internal static void SaveAssembly()
        {
            _assemblyBuilder.Save(_moduleName);
        }

        public static Predicate<Type> DeepProxyFilter
        {
            set
            {
                lock(_initLock)
                {
                    RequiresNotInitLocked();
                    _deepProxyFilter = value ?? _noDeepProxy;
                }
            }
        }

        public static void SetDeepProxyAttribute<TA>()
            where TA : Attribute
        {
            DeepProxyFilter = t => t.GetCustomAttributes(typeof (TA), true).Length != 0;
        }

        public static T NewProxy<T>(T target) where T : class 
        {
            return Generator<T>.NewProxy(target);
        }

        public static T GetProxy<T>(T target) where T : class 
        {
            return Generator<T>.GetProxy(target);
        }

        private static void RegisterCache(IWeakCollection cache)
        {
            lock (_caches) _caches.Add(cache);
        }

        private static void CleanupCaches(object dummy)
        {
            if(Interlocked.CompareExchange(ref _isCleanupInProcess, 1, 0)!=0) return;
            try
            {
                IWeakCollection[] caches;
                lock (_caches)
                {
                    caches = _caches.ToArray();
                }
                foreach (var cache in caches)
                {
                    lock (cache)
                    {
                        cache.RemoveCollectedEntries();
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isCleanupInProcess, 0);
            }
        }

        struct Generator<T> where T : class 
        {
            static readonly Converter<T, T> _wrap;
            static readonly Type _type;

            private static readonly WeakDictionary<T, T> _cache
                = new WeakDictionary<T, T>();

            private IGenerator g;
            private IField wrapped;
            private IClass wrapper;


            static Generator()
            {
                lock(_initLock) _isInitLocked = true;

                RegisterCache(_cache);
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

            public static T GetProxy(T target)
            {
                if (target == null) return null;
                lock(_cache)
                {
                    T proxy;
                    if(!_cache.TryGetValue(target, out proxy))
                    {
                        proxy = NewProxy(target);
                        _cache[target] = proxy;
                    }
                    return proxy;
                }
            }

            private IClass DefineWrapper()
            {
                Type @interface = typeof (T);
                wrapper = g.Class("ChangeCheckerFor" + @interface.Name).In(_namespace).Inherits(_baseClassType).Implements(@interface);
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
                            c.Call(wrapper.This.Invoke(_onPropertyChanged, g.Const(pi.Name)));
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
                        //TODO: we are doing indexers too, should we? No! we shouldn't
                        //if (ReferenceEquals(value, _ComponentProperty)) return;
                        c.If(c.ReferenceEquals(setter.Value, backingField));
                        {
                            c.Return();
                        }
                        c.End();
                        //if (ReferenceEquals(value, _ComponentProperty._wrapped)) return;
                        c.If(c.ReferenceEquals(setter.Value, backingField.Field("_wrapped")));
                        {
                            c.Return();
                        }
                        c.End();

                        // var p = _wrapped.ComponentProperty;
                        var proxy = c.Variable(proxyType, "proxy");
                        c.Assign(proxy, setter.Value.As(proxyType));

                        // if (proxy == null)
                        c.If(c.IsNull(proxy));
                        {
                            //_wrapped.ComponentProperty = value;
                            c.Assign(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value);
                        }
                        c.Else();
                        {
                            //_wrapped.ComponentProperty = proxy._wrapped;
                            c.Assign(wrapped.Property(pi, setter.Args.AsOperands()), proxy.Field("_wrapped"));
                            //_ComponentProperty = proxy;
                            c.Assign(backingField, proxy);
                        }
                        c.End();

                        //FirePropertyChanged("ComponentProperty");
                        c.Call(wrapper.This.Invoke(_onPropertyChanged, g.Const(pi.Name)));
                    }
                }
            }
        }

        
    }

    public interface ICompositeProxy<T>
    {
        T Target { get; }
    }

    internal class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) 
                propertyChanged(this, new PropertyChangedEventArgs("propertyName"));
        }
    }
}
