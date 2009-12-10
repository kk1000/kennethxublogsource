using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using CodeSharp.Emit;

namespace CodeSharp.Proxy.NPC
{
    /// <summary>
    /// Factory class to create composite based proxy that implements
    /// <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    internal static class Factory
    {
        #region Private Fields

        #region Assembly Related
        const string _moduleName = "NotifyPropertyChangedFactory.dll";
        const string _namespace = "NPC";
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly ModuleBuilder _moduleBuilder;
        #endregion

        #region Initialization Related
        private static readonly Predicate<Type> _noDeepProxy = t => false;
        private static readonly object _initLock = new object();
        private static bool _isInitialized;
        private static MethodInfo _onPropertyChanged;
        private static Type _baseClassType;
        private static Predicate<Type> _deepProxyFilter = _noDeepProxy; 
        #endregion

        #region Cache Related
		private static readonly List<IWeakCollection> _caches = new List<IWeakCollection>();
        private static int _isCleanupInProcess;
        #pragma warning disable 169
        private static readonly Timer _cacheCleanupTimer = new Timer(CleanupCaches, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        #pragma warning restore 169 
	    #endregion

        private static readonly Dictionary<string, MethodInfo> _getProxyMethods = new Dictionary<string, MethodInfo>();
        private static readonly Dictionary<string, MethodInfo> _getTargetMethods = new Dictionary<string, MethodInfo>();

        #endregion

        static Factory()
        {
            AssemblyName an = new AssemblyName { Name = _moduleName };
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            _assemblyBuilder = ab;
            _moduleBuilder = ab.DefineDynamicModule(_moduleName, _moduleName);
            SetBaseClass(typeof(NotifyPropertyChangedFactory.ProxyBase), NotifyPropertyChangedFactory.DefaultOnPropertyChangedMethodName);
            DiscoverMethodInfo();
        }

        /// <summary>
        /// Save all generated proxy types to an assembly file.
        /// </summary>
        internal static void SaveAssembly()
        {
            _assemblyBuilder.Save(_moduleName);
        }
        
        #region Initialization Related Members

        /// <summary>
        /// Set the base class for all the generated proxies.
        /// </summary>
        /// <remarks>
        /// <paramref name="baseClassType"/> must be a class that is not sealed and
        /// implements <see cref="INotifyPropertyChanged"/>. It also must has a
        /// non-abstract method that take one string parameter. The name of the
        /// method is specified by <paramref name="onPropertyChangeMethod"/>.
        /// </remarks>
        /// <param name="baseClassType">
        /// Type of the base class.
        /// </param>
        /// <param name="onPropertyChangeMethod">
        /// The name of the method to raise the 
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </param>
        internal static void SetBaseClass(Type baseClassType, string onPropertyChangeMethod)
        {
            if (baseClassType == null) throw new ArgumentNullException("baseClassType");
            if (onPropertyChangeMethod == null) throw new ArgumentNullException("onPropertyChangeMethod");
            lock (_initLock)
            {
                RequiresNotInitialized();
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

        /// <summary>
        /// The filter to indicate if the factory should generate proxy for
        /// types that are used as parameter or return value of type of which
        /// a proxy is being generated.
        /// </summary>
        /// <remarks>
        /// This property can only be set before any proxy is generated.
        /// Otherwise <see cref="InvalidOperationException"/> is thrown.
        /// </remarks>
        internal static Predicate<Type> DeepProxyFilter
        {
            set
            {
                lock (_initLock)
                {
                    RequiresNotInitialized();
                    _deepProxyFilter = value ?? _noDeepProxy;
                }
            }
        }

        private static void RequiresNotInitialized()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException(
                    "This method can only be called before any proxy object are created.");
            }
        }

        #endregion

        #region Cache Related Members
        
        private static void RegisterCache(IWeakCollection cache)
        {
            lock (_caches) _caches.Add(cache);
        }

        private static void CleanupCaches(object dummy)
        {
            if (Interlocked.CompareExchange(ref _isCleanupInProcess, 1, 0) != 0) return;
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

        #endregion

        private static void DiscoverMethodInfo()
        {
            var members = typeof(NotifyPropertyChangedFactory).GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in members.Where(m => m.MemberType == MemberTypes.Method))
            {
                switch (method.Name)
                {
                    case "GetProxy":
                        _getProxyMethods[method.GetParameters()[0].ParameterType.ToString()] = method;
                        break;
                    case "GetTarget":
                        _getTargetMethods[method.GetParameters()[0].ParameterType.ToString()] = method;
                        break;
                }
            }

        }
        private static string GetDeepProxyKey(Type type)
        {
            string key = "T";
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(IDictionary<,>))
                {
                    type = type.GetGenericArguments()[1];
                    key = genericType.ToString();
                }
                else if (
                    genericType == typeof(IList<>) ||
                    genericType == typeof(ICollection<>) ||
                    genericType == typeof(IEnumerable<>) ||
                    genericType == typeof(IEnumerator<>))
                {
                    type = type.GetGenericArguments()[0];
                    key = genericType.ToString();
                }
            }
            return _deepProxyFilter(type) ? key : null;
        }
        
        /// <summary>Actual proxy generator.</summary>
        internal struct Generator<T> where T : class 
        {
            static readonly Converter<T, T> _newProxy;
            static readonly Converter<T, T> _getTarget;
            static readonly Type _type;

            private static readonly WeakDictionary<T, T> _cache
                = new WeakDictionary<T, T>();

            private IGenerator g;
            private IField _target;
            private IClass _proxy;

            static Generator()
            {
                lock(_initLock) _isInitialized = true;

                RegisterCache(_cache);
                var @interface = typeof(T);
                if (!@interface.IsInterface)
                    throw new InvalidOperationException(@interface + " is not an interface type.");
                Emitter e = new Emitter(_moduleBuilder);
                _type = e.Generate(new Generator<T>{g=e}.DefineProxy());
                //var factoryMethod = _type.GetMethod("CreateProxy", new[] {@interface});
                //_newProxy = (Converter<T, T>) Delegate.CreateDelegate(typeof(Converter<T, T>), factoryMethod);

                var constructor = _type.GetConstructor(new[] { @interface });
                _newProxy = x => (T)constructor.Invoke(new object[] { x });
                _getTarget = (Converter<T, T>)Delegate.CreateDelegate(typeof(Converter<T, T>), _type.GetMethod("GetTarget"));
            }

            internal static T NewProxy(T target)
            {
                return _newProxy(target);
            }

            internal static T GetProxy(T target)
            {
                if (target == null) return null;
                if (target.GetType() == _type) return target;

                lock (_cache)
                {
                    T proxy;
                    if(!_cache.TryGetValue(target, out proxy))
                    {
                        proxy = _newProxy(target);
                        _cache[target] = proxy;
                    }
                    return proxy;
                }
            }

            internal static T GetTarget(T proxy)
            {
                return _getTarget(proxy);
            }

            private IClass DefineProxy()
            {
                Type @interface = typeof (T);
                _proxy = g.Class("Proxy" + @interface.Name).In(_namespace).Inherits(_baseClassType).Implements(@interface).Public;
                {
                    _target = _proxy.Field(@interface, "_target").Internal.ReadOnly;

                    var ctor = _proxy.Constructor(g.Arg(@interface, "target")).Public;
                    using (var c = ctor.Code())
                    {
                        c.Assign(_target, ctor.Args[0]);
                    }

                    GenGetTarget();

                    GenInterfaceImpl(@interface);
                }
                return _proxy;
            }

            private void GenGetTarget()
            {
                var getTarget = _proxy.Method(typeof (T), "GetTarget", g.Arg(typeof (T), "proxy")).Public.Static;
                using (var c = getTarget.Code())
                {
                    var confirmProxy = c.Variable(_proxy, "confirmProxy", getTarget.Args[0].As(_proxy));
                    c.If(c.IsNull(confirmProxy));
                    {
                        c.Return(getTarget.Args[0]);
                    }
                    c.End();
                    c.Return(confirmProxy.Field(_target));
                }
            }

            private void GenInterfaceImpl(Type @interface)
            {
                foreach (MemberInfo info in @interface.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                {
                    switch (info.MemberType)
                    {
                        case MemberTypes.Method:
                            MethodInfo mi = (MethodInfo)info;
                            if (!mi.IsSpecialName)
                            {
                                GenMethod(mi);
                            }
                            break;
                        case MemberTypes.Property:
                            PropertyInfo pi = (PropertyInfo)info;
                            if (pi.GetIndexParameters().Length != 0)
                            {
                                GenIndexer(pi);
                            }
                            else
                            {
                                var key = GetDeepProxyKey(pi.PropertyType);
                                if (key != null) GenDeepProxiedProperty(pi, key);
                                else GenProperty(pi);
                            }
                            break;
                        default:
                            Console.WriteLine(info + " : " + info.MemberType);
                            break;
                    }
                }
            }

            private void GenMethod(MethodInfo mi)
            {
                var method = _proxy.Method(mi).Public;
                using (var code = method.Code())
                {
                    code.Return(_target.Invoke(mi, method.Args.AsOperands()));
                }
            }

            private void GenIndexer(PropertyInfo pi)
            {
                var property = _proxy.Property(pi).Public;
                if (pi.CanRead)
                {
                    var getter = property.Getter();
                    using (var c = getter.Code())
                    {
                        c.Return(_target.Property(pi, getter.Args.AsOperands()));
                    }
                }
                if (pi.CanWrite)
                {
                    var setter = property.Setter();
                    using (var c = setter.Code())
                    {
                        c.Assign(_target.Property(pi, setter.Args.AsOperands()), setter.Value);
                    }
                }
            }

            private void GenProperty(PropertyInfo pi)
            {
                // public int IntProperty
                var property = _proxy.Property(pi).Public;
                {
                    if (pi.CanRead)
                    {
                        var getter = property.Getter();
                        using (var c = getter.Code())
                            // get
                        {
                            // return _target.IntProperty;
                            c.Return(_target.Property(pi));
                        }
                    }
                    if (pi.CanWrite)
                    {
                        var setter = property.Setter();
                        using (var c = setter.Code())
                            // set
                        {
                            // if (_target.IntProperty != value)
                            c.If(c.AreNotEqual(_target.Property(pi, setter.Args.AsOperands()), setter.Value));
                            {
                                // _target.IntProperty = value;
                                c.Assign(_target.Property(pi), setter.Value);
                                // FirePropertyChanged("IntProperty");
                                c.Call(_proxy.This.Invoke(_onPropertyChanged, g.Const(pi.Name)));
                            }
                            c.End();
                        }
                    }
                }
            }

            private void GenDeepProxiedProperty(PropertyInfo pi, string key)
            {
                var propertyType = pi.PropertyType;
                var typeArguments = key == "T" ? new[] {propertyType} : propertyType.GetGenericArguments();
                var backingField = _proxy.Field(propertyType, "_" + pi.Name);
                MethodInfo getTargetMethod = _getTargetMethods[key].MakeGenericMethod(typeArguments);
                MethodInfo getProxyMethod = _getProxyMethods[key].MakeGenericMethod(typeArguments);

                // public IValueComponent ComponentProperty
                var property = _proxy.Property(pi).Public;
                if (pi.CanRead)
                {
                    var getter = property.Getter();
                    using (var c = getter.Code())
                    // get
                    {
                        // var p = _target.ComponentProperty;
                        var p = c.Variable(propertyType, "p");
                        c.Assign(p, _target.Property(pi, getter.Args.AsOperands()));

                        // if (p == null)
                        c.If(c.IsNull(p));
                        {
                            var @null = g.Null(propertyType);
                            // _ComponentProperty = null;
                            c.Assign(backingField, @null);
                            // reurn null;
                            c.Return(@null);
                        }
                        c.End();

                        // if (_ComponentProperty == null || !ReferenceEquals((NotifyPropertyChangedFactory.GetTarget(_ComponentProperty), p))
                        c.If(c.Or(c.IsNull(backingField), c.NotReferenceEquals(c.Invoke(getTargetMethod, backingField), p)));
                        {
                            // _ComponentProperty = NotifyPropertyChangedFactory.GetProxy(p);
                            c.Assign(backingField, c.Invoke(getProxyMethod, p));
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
                    // set
                    {
                        // var newTarget = NotifyPropertyChangedFactory.GetTarget(value);
                        var newTarget = c.Variable(propertyType, "newTarget", c.Invoke(getTargetMethod, setter.Value));
                        // if (!ReferenceEquals(_target.ComponentProperty, newTarget))
                        c.If(c.NotReferenceEquals(_target.Property(pi), newTarget));
                        {
                            // _target.ComponentProperty = newTarget;
                            c.Assign(_target.Property(pi), newTarget);
                            // _ComponentProperty = NotifyPropertyChangedFactory.GetProxy(value);
                            c.Assign(backingField, c.Invoke(getProxyMethod, setter.Value));
                            //this.FirePropertyChanged("ComponentProperty");
                            c.Call(_proxy.This.Invoke(_onPropertyChanged, g.Const(pi.Name)));
                        }
                        c.End();
                    }
                }
                // end of ComponentProperty
            }
        }
    }
}
