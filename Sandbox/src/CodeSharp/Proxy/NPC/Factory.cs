#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// <author>Kenneth Xu</author>
    internal static class Factory
    {
        #region Private Fields

        #region Assembly Related
        const string _moduleName = "NotifyPropertyChangeFactory.dll";
        const string _namespace = "NPC";
        private static AssemblyBuilder _assemblyBuilder;
        private static ModuleBuilder _moduleBuilder;
        private static int _customizationRevision;
        #endregion

        #region Initialization Related
        private static readonly Predicate<Type> _alwaysFalse =  t => false;
        private static readonly object _initLock = new object();

        private static bool _isInitialized;
        private static Type _defaultBaseType;
        private static MethodInfo _defaultPropertyChangedRaiser;
        private static Type _markingAttribute;
        private static Type _raiserAttribute;

        private static Predicate<Type> _isProxyTargetType = _alwaysFalse;
        private static Converter<object, Type> _getBaseTypeFromMarkingAttribute;
        private static Converter<object, string> _getPropertyChangedRaiserFromRaiserAttribute;
        private static Converter<object, string> _getPropertyChangedRaiserFromMarkingAttribute;
        #endregion

        #region Cache Related
		private static List<IWeakCollection> _caches = new List<IWeakCollection>();
        private static int _isCleanupInProcess;
        #pragma warning disable 169
        private static readonly Timer _cacheCleanupTimer;
        #pragma warning restore 169 
	    #endregion

        private static readonly Dictionary<string, FactoryPair> _factoryPairs = 
            new Dictionary<string, FactoryPair>();


        #endregion

        static Factory()
        {
            Init(_moduleName);
            DiscoverMethodInfo();
            _cacheCleanupTimer = new Timer(CleanupCaches, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        private static void Init(string moduleName)
        {
            var revision = _customizationRevision;
            AssemblyName an = new AssemblyName { Name = moduleName };
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            _assemblyBuilder = ab;
            _moduleBuilder = ab.DefineDynamicModule(moduleName, moduleName);
            _isInitialized = false;
            SetBaseType(typeof(NotifyPropertyChangeBase), NotifyPropertyChangeFactory.DefaultOnPropertyChangedMethodName);
            SetMarkingAttribute<NotifyPropertyChangeAttribute>(a => a.BaseType);
            SetEventRaiserAttribute<OnPropertyChangeAttribute>(a => a.OnPropertyChangedMethod);
            _caches = new List<IWeakCollection>();
            _customizationRevision = revision;
        }

        internal static void Reset(bool cleanProxy)
        {
            var name = MakeModuleName();

            if (name == _assemblyBuilder.GetName().Name)
            {
                if (!cleanProxy || !_isInitialized) return;
                ++_customizationRevision;
                name = MakeModuleName();
            }
            var caches = _caches;
            Init(name);
            foreach (var cache in caches)
            {
                var type = cache.GetType().GetGenericArguments()[0];
                var generatorType = typeof (Generator<>).MakeGenericType(type);
                generatorType.GetMethod("Reset", BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public).Invoke(null, null);
            }
        }

        private static string MakeModuleName()
        {
            return _customizationRevision == 0 ? _moduleName :
                Path.GetFileNameWithoutExtension(_moduleName) + 
                _customizationRevision + 
                Path.GetExtension(_moduleName);
        }


        /// <summary>
        /// Save all generated proxy types to an assembly file.
        /// </summary>
        internal static void SaveAssembly()
        {
            _assemblyBuilder.Save(_moduleName);
        }
        
        #region Initialization Related Members

        /// <seealso cref="NotifyPropertyChangeFactory.SetBaseType{T}(string)"/>
        internal static void SetBaseType(Type baseType, string onPropertyChangedMethod)
        {
            if (baseType == null) 
                throw new ArgumentNullException("baseType");
            if (onPropertyChangedMethod == null) 
                throw new ArgumentNullException("onPropertyChangedMethod");
            if (baseType.IsGenericTypeDefinition && baseType.GetGenericArguments().Length > 1)
            {
                throw new ArgumentException(
                    "Only one type parameter is allowed for open generic base type.", 
                    "baseType");
            }
            ValidateBaseType(baseType);
            var raiser = GetOnPropertyChangedMethod(baseType, onPropertyChangedMethod);
            if (_defaultBaseType == baseType || _defaultPropertyChangedRaiser == raiser) return;
            lock (_initLock)
            {
                RequiresNotInitialized();
                _customizationRevision++;
                _defaultBaseType = baseType;
                _defaultPropertyChangedRaiser = raiser;
            }
        }

        private static void ValidateBaseType(Type baseType)
        {
            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(baseType)) 
                throw new ArgumentException("Must implement INotifyPropertyChanged.", "baseType");
            if (!baseType.IsPublic && !baseType.IsNestedPublic)
                throw new ArgumentException("Must be public type.", "baseType");
            if (baseType.IsInterface || baseType.IsValueType || typeof(Delegate).IsAssignableFrom(baseType))
                throw new ArgumentException("Requires class type, must not be delegate, enum, struct or interface", "baseType");
            if (baseType.IsSealed)
                throw new ArgumentException("Must not be sealed.", "baseType");
        }

        /// <seealso cref="NotifyPropertyChangeFactory.SetMarkingAttribute{TA}(Converter{TA,Type})"/>
        internal static void SetMarkingAttribute<TA>(Converter<TA, Type> baseType)
            where TA : Attribute
        {
            lock(_initLock)
            {
                RequiresNotInitialized();
                _customizationRevision++;
                _markingAttribute = typeof(TA);
                _isProxyTargetType = t => t.GetCustomAttributes(typeof(TA), true).Length != 0;
                _getBaseTypeFromMarkingAttribute = baseType == null ? (Converter<object, Type>)null : o => baseType((TA)o);
                _getPropertyChangedRaiserFromMarkingAttribute = null;
                var changedName = typeof (TA).GetProperty("OnPropertyChangedMethodName", typeof (string));
                if (changedName == null) return;
                var getter = changedName.GetGetMethod();
                if (getter == null) return;
                var getRaiserName = (Converter<TA, string>) Delegate.CreateDelegate(typeof (Converter<TA, string>), getter);
                if (getRaiserName == null) return;
                _getPropertyChangedRaiserFromMarkingAttribute = o => getRaiserName((TA) o);
            }
        }

        /// <seealso cref="NotifyPropertyChangeFactory.SetEventRaiserAttribute{TA}(Converter{TA,string})"/>
        internal static void SetEventRaiserAttribute<TA>(Converter<TA, string> onPropertyChanged)
            where TA : Attribute
        {
            if (onPropertyChanged == null) throw new ArgumentNullException("onPropertyChanged");
            lock(_initLock)
            {
                RequiresNotInitialized();
                _customizationRevision++;
                _raiserAttribute = typeof(TA);
                _getPropertyChangedRaiserFromRaiserAttribute = o => onPropertyChanged((TA) o);
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

        private static MethodInfo GetOnPropertyChangedMethod(Type baseType, string onPropertyChangedMethod)
        {
            var onPropertyChanged = baseType.GetMethod(
                onPropertyChangedMethod, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, new[] { typeof(string) }, null);
            if (onPropertyChanged == null || 
                !(onPropertyChanged.IsPublic || onPropertyChanged.IsFamily) || 
                onPropertyChanged.IsAbstract)
            {
                throw new ArgumentException(
                    string.Format("Public or protected method {0}.{1}(string) doesn't exist.",
                                  baseType.FullName, onPropertyChangedMethod));
            }
            return onPropertyChanged;
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

        /// <summary>
        /// Initialize the GetProxy and GetTarget method pair to be used by
        /// <see cref="GetFactoryPair"/> method.
        /// </summary>
        private static void DiscoverMethodInfo()
        {
            var members = typeof(NotifyPropertyChangeFactory).GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in members.Where(m => m.MemberType == MemberTypes.Method))
            {
                Action<FactoryPair, MethodInfo> setter;
                switch (method.Name)
                {
                    case "GetProxy":
                        setter = (p, m) => p.GetProxy = m;
                        break;
                    case "GetTarget":
                        setter = (p, m) => p.GetTarget = m;
                        break;
                    default:
                        setter = null;
                        break;
                }
                if (setter != null)
                {
                    var key = method.GetParameters()[0].ParameterType.ToString();
                    FactoryPair pair;
                    if(!_factoryPairs.TryGetValue(key, out pair))
                    {
                        pair = new FactoryPair();
                        _factoryPairs[key] = pair;
                    }
                    setter(pair, method);
                }
            }

        }

        /// <summary>
        /// This gives you GetProxy and GetTarget method pair if the type is
        /// proxy-able. Otherwise return null.
        /// </summary>
        private static FactoryPair GetFactoryPair(Type type)
        {
            string key = null;
            if (type.IsByRef) type = type.GetElementType();
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(IDictionary<,>))
                {
                    if (!_isProxyTargetType(type.GetGenericArguments()[1])) return null;
                    key = genericType.ToString();
                }
                else if (
                    genericType == typeof(IList<>) ||
                    genericType == typeof(ICollection<>) ||
                    genericType == typeof(IEnumerable<>) ||
                    genericType == typeof(IEnumerator<>))
                {
                    if (!_isProxyTargetType(type.GetGenericArguments()[0])) return null;
                    key = genericType.ToString();
                }
            }
            if (key == null)
            {
                if(!_isProxyTargetType(type)) return null;
                key = "T";
            }

            var typeArguments = key == "T" ? new[] { type } : type.GetGenericArguments();
            var pair = _factoryPairs[key];

            return new FactoryPair
                       {
                           GetProxy = pair.GetProxy.MakeGenericMethod(typeArguments),
                           GetTarget = pair.GetTarget.MakeGenericMethod(typeArguments),
                       };
        }
        
        /// <summary>Actual proxy generator.</summary>
        internal class Generator<T> where T : class 
        {
            static Converter<T, T> _newProxy;
            static Converter<T, T> _getTarget;
            static Type _proxyType;
            static WeakDictionary<T, T> _cache;

            private IGenerator g;
            private IField _target;
            private IClass _proxy;
            private Type _baseType;
            private Type _interfaceType;
            private MethodInfo _propertyChangedRaiser;
            private Dictionary<MethodInfo, MethodInfo> _interfaceToBaseMethods =
                new Dictionary<MethodInfo, MethodInfo>();
            private readonly Dictionary<string, MethodInfo> _propertyChangedRaiserCache = 
                new Dictionary<string, MethodInfo>();

            static Generator()
            {
                Init();
            }

            // So nobody except this class itself can instanciate.
            private Generator() {}

            // this is just for unit test only.
            internal static void Reset()
            {
                _proxyType = null;
            }

            /// <seealso cref="NotifyPropertyChangeFactory.NewProxy{T}(T)"/>
            internal static T NewProxy(T target)
            {
                if (target == null) return null;
                if (_proxyType == null) Init();
                return _newProxy(target);
            }

            /// <seealso cref="NotifyPropertyChangeFactory.GetProxy{T}(T)"/>
            internal static T GetProxy(T target)
            {
                if (target == null) return null;
                if (_proxyType == null) Init();
                if (target.GetType() == _proxyType) return target;

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

            /// <seealso cref="NotifyPropertyChangeFactory.GetTarget{T}(T)"/>
            internal static T GetTarget(T proxy)
            {
                if (_proxyType == null) Init();
                return _getTarget(proxy);
            }

            private static void Init()
            {
                lock (_initLock) _isInitialized = true;

                _cache = new WeakDictionary<T, T>();
                RegisterCache(_cache);
                var @interface = typeof(T);
                if (!@interface.IsInterface)
                    throw new InvalidOperationException(@interface + " is not an interface type.");
                Emitter e = new Emitter(_moduleBuilder);
                _proxyType = e.Generate(new Generator<T> { g = e }.DefineProxy());

                var constructor = _proxyType.GetConstructor(new[] { @interface });
                var dynamicConstructor = new DynamicMethod(
                    "NewProxyDynamic", @interface, new[] { @interface }, _proxyType);
                var il = dynamicConstructor.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);
                _newProxy = (Converter<T, T>)dynamicConstructor.CreateDelegate(typeof(Converter<T, T>));

                _getTarget = (Converter<T, T>)Delegate.CreateDelegate(
                                                  typeof(Converter<T, T>), _proxyType.GetMethod("GetTarget"));
            }

            private IClass DefineProxy()
            {
                _interfaceType = typeof (T);
                ProcessMarkingAttribute(_interfaceType);
                MapMethods();
                // public class ProxyIValueObject : ChangeTrackerBase, IValueObject
                _proxy = g.Class("Proxy" + _interfaceType.Name).In(_namespace).Inherits(_baseType).Implements(_interfaceType).Public;
                {
                    // private readonly IValueObject _target;
                    _target = _proxy.Field(_interfaceType, "_target").Internal.ReadOnly;

                    ImplConstructor();

                    var pi = _baseType.GetProperty(
                        "Target", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if(pi != null && pi.PropertyType.IsAssignableFrom(_interfaceType) && pi.GetGetMethod(true).IsAbstract && pi.GetSetMethod(true) == null)
                    {
                        ImplTargetProperty(pi);
                    }

                    ImplGetTarget();

                    ImplInterface();
                }
                return _proxy;
            }

            private void ProcessMarkingAttribute(Type @interface)
            {
                var defaultBaseType = _defaultBaseType;
                if (defaultBaseType.IsGenericTypeDefinition)
                {
                    defaultBaseType = defaultBaseType.MakeGenericType(_interfaceType);
                }
                // default to factory default
                _baseType = defaultBaseType;
                _propertyChangedRaiser = _defaultPropertyChangedRaiser;
                // end if there is no delegate to get additional information.
                if (_getBaseTypeFromMarkingAttribute == null && 
                    _getPropertyChangedRaiserFromMarkingAttribute == null) return;
                // otherwise try to get the attribute.
                var attrs = @interface.GetCustomAttributes(_markingAttribute, false);
                // end if there is no marking attribute.
                if (attrs.Length == 0) return;
                // if there is a different type specified in marking attribute, use it.
                if (_getBaseTypeFromMarkingAttribute != null)
                {
                    var baseType = _getBaseTypeFromMarkingAttribute(attrs[0]);
                    if(baseType!=null)
                    {
                        ValidateBaseType(baseType);
                        _baseType = baseType;
                    }
                }
                // if there is a diffrent property changed event raiser specified, use it.
                string raiserName = null;
                if (_getPropertyChangedRaiserFromMarkingAttribute != null)
                {
                    raiserName = _getPropertyChangedRaiserFromMarkingAttribute(attrs[0]);
                }
                if (raiserName == null) raiserName = _propertyChangedRaiser.Name;
                if (_baseType != defaultBaseType || raiserName != _propertyChangedRaiser.Name)
                {
                    _propertyChangedRaiser = GetOnPropertyChangedMethod(_baseType, raiserName);
                }
            }

            /// <summary>
            /// Maps the methods between those in the interface and those in 
            /// the base class. The information will be used to determine what
            /// and how to implement in the proxy.
            /// </summary>
            private void MapMethods()
            {
                MapMethods(_interfaceType);
                foreach (var type in _interfaceType.GetInterfaces())
                {
                    MapMethods(type);
                }
            }

            private void MapMethods(Type interfaceType)
            {
                if(_baseType.GetInterfaces().Contains(interfaceType))
                {
                    var map = _baseType.GetInterfaceMap(interfaceType);
                    var count = map.InterfaceMethods.Length;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        _interfaceToBaseMethods[map.InterfaceMethods[i]] = map.TargetMethods[i];
                    }
                    return;
                }
                var interfaceMethods = interfaceType.GetMethods();
                foreach (var interfaceMethod in interfaceMethods)
                {
                    foreach (var methodInfo in _baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (MethodSignatureAreSame(methodInfo, interfaceMethod))
                        {
                            _interfaceToBaseMethods[interfaceMethod] = methodInfo;
                        }
                    }
                }
            }

            /// <summary>
            /// To compare if the signatures and return types of two given 
            /// methods are the same.
            /// </summary>
            private static bool MethodSignatureAreSame(MethodInfo m1, MethodInfo m2)
            {
                if (m1.Name != m2.Name || m1.ReturnType != m2.ReturnType) return false;
                var p1 = m1.GetParameters(); 
                var p2 = m2.GetParameters();
                if (p1.Length != p2.Length) return false;
                for (int i = p1.Length - 1; i >= 0; i--)
                {
                    if (p1[i].ParameterType != p2[i].ParameterType) return false;
                }
                return true;
            }

            private IEnumerable<MemberInfo> GetInterfaceMembers()
            {
                foreach (var info in _interfaceType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    yield return info;
                foreach (var type in _interfaceType.GetInterfaces())
                {
                    foreach (var info in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                        yield return info;
                }
            }

            private void ImplConstructor()
            {
                // public ProxyIValueObject(IValueObject target)
                var ctor = _proxy.Constructor(g.Arg(_interfaceType, "target")).Public;
                using (var c = ctor.Code())
                {
                    // _target = target;
                    c.Assign(_target, ctor.Args[0]);
                }
            }

            private void ImplTargetProperty(PropertyInfo pi)
            {
                // protected override Target
                var property = _proxy.Property(pi).Override(pi);
                using (var c = property.Getter().Code())
                // get
                {
                    // return _target;
                    c.Return(_target);
                }
            }

            private void ImplGetTarget()
            {
                // public static IValueObject GetTarget(IValueObject proxy)
                var getTarget = _proxy.Method(typeof (T), "GetTarget", g.Arg(typeof (T), "proxy")).Public.Static;
                using (var c = getTarget.Code())
                {
                    // var confirmProxy = proxy as ProxyIValueObject;
                    var confirmProxy = c.Variable(_proxy, "confirmProxy", getTarget.Args[0].As(_proxy));
                    // if(confirmProxy == null)
                    c.If(c.IsNull(confirmProxy));
                    {
                        // return proxy;
                        c.Return(getTarget.Args[0]);
                    }
                    c.End();
                    // return confirmProxy._target;
                    c.Return(confirmProxy.Field(_target));
                }
            }

            private void ImplInterface()
            {
                foreach (MemberInfo info in GetInterfaceMembers())
                {
                    switch (info.MemberType)
                    {
                        case MemberTypes.Method:
                            MethodInfo mi = (MethodInfo)info;
                            if (!mi.IsSpecialName)
                            {
                                ImplMethod(mi);
                            }
                            break;
                        case MemberTypes.Property:
                            PropertyInfo pi = (PropertyInfo)info;
                            if (pi.GetIndexParameters().Length != 0)
                            {
                                ImplIndexer(pi);
                            }
                            else
                            {
                                var pair = GetFactoryPair(pi.PropertyType);
                                if (pair != null) ImplProxyTargetProperty(pi, pair);
                                else ImplProperty(pi);
                            }
                            break;
                    }
                }
            }

            private void ImplMethod(MethodInfo mi)
            {
                MethodInfo mBase;
                if (!NeedsMethod(mi, out mBase)) return;
                var method = _proxy.Method(mi).Public;
                if (mBase != null) method.Override(mi);
                using (var c = method.Code())
                {
                    ProxyInvoke(c, mi.ReturnType, method.Args, (r, p) => _target.Invoke(mi, p));
                }
            }

            private void ImplIndexer(PropertyInfo pi)
            {
                ImplProperty(
                    pi,
                    getter =>
                    {
                        using (var c = getter.Code())
                        {
                            ProxyInvoke(c, pi.PropertyType, getter.Args, 
                                (r, p) => _target.Property(pi, p));
                        }
                    },
                    setter =>
                    {
                        using (var c = setter.Code())
                        {
                            ProxyInvoke(c, pi.PropertyType, setter.Args,
                                (r, p) =>
                                    {
                                        IOperand value = setter.Value;
                                        if (r != null) value = c.Invoke(r.GetTarget, value);
                                        c.Assign(_target.Property(pi, p), value);
                                        return null;
                                    });
                        }
                    });
            }

            private void ImplProperty(PropertyInfo pi)
            {
                var propertychangedRaiser = FindOnPropertyChangedMethod(pi);
                // public int IntProperty
                ImplProperty(
                    pi,
                    getter =>
                        {
                            // get
                            using (var c = getter.Code())
                            {
                                // return _target.IntProperty;
                                c.Return(_target.Property(pi));
                            }
                        },
                    setter =>
                        {
                            // set
                            using (var c = setter.Code())
                            {
                                if (propertychangedRaiser == null)
                                {
                                    // _target.IntProperty = value;
                                    c.Assign(_target.Property(pi), setter.Value);
                                    return;
                                }
                                // if (_target.IntProperty != value)
                                c.If(c.AreNotEqual(_target.Property(pi, setter.Args.AsOperands()), setter.Value));
                                {
                                    // _target.IntProperty = value;
                                    c.Assign(_target.Property(pi), setter.Value);
                                    // FirePropertyChanged("IntProperty");
                                    c.Call(_proxy.This.Invoke(propertychangedRaiser, g.Const(pi.Name)));
                                }
                                c.End();
                            }
                        });
            }

            private void ImplProxyTargetProperty(PropertyInfo pi, FactoryPair pair)
            {
                var propertychangedRaiser = FindOnPropertyChangedMethod(pi);
                var propertyType = pi.PropertyType;

                // private IValueComponent _ComponentProperty;
                var backingField = _proxy.Field(propertyType, "_" + pi.Name);

                // public IValueComponent ComponentProperty
                ImplProperty(
                    pi,
                    getter =>
                        {
                            // get
                            using (var c = getter.Code())
                            {
                                // var p = _target.ComponentProperty;
                                var p = c.Variable(propertyType, "p", _target.Property(pi, getter.Args.AsOperands()));

                                // if (p == null)
                                c.If(c.IsNull(p));
                                {
                                    // _ComponentProperty = null;
                                    c.Assign(backingField, g.Null(propertyType));
                                    // return null;
                                    c.Return(g.Null(propertyType));
                                }
                                c.End();

                                // if (_ComponentProperty == null || !ReferenceEquals((NotifyPropertyChangeFactory.GetTarget(_ComponentProperty), p))
                                c.If(c.Or(c.IsNull(backingField),
                                          c.NotReferenceEquals(c.Invoke(pair.GetTarget, backingField), p)));
                                {
                                    // _ComponentProperty = NotifyPropertyChangeFactory.GetProxy(p);
                                    c.Assign(backingField, c.Invoke(pair.GetProxy, p));
                                }
                                c.End();

                                // return _ComponentProperty;
                                c.Return(backingField);
                            }
                        },
                    setter =>
                        {
                            // set
                            using (var c = setter.Code())
                            {
                                // var newTarget = NotifyPropertyChangeFactory.GetTarget(value);
                                var newTarget = c.Variable(propertyType, "newTarget",
                                                           c.Invoke(pair.GetTarget, setter.Value));
                                // if (!ReferenceEquals(_target.ComponentProperty, newTarget))
                                c.If(c.NotReferenceEquals(_target.Property(pi), newTarget));
                                {
                                    // _target.ComponentProperty = newTarget;
                                    c.Assign(_target.Property(pi), newTarget);
                                    // _ComponentProperty = NotifyPropertyChangeFactory.GetProxy(value);
                                    c.Assign(backingField, c.Invoke(pair.GetProxy, setter.Value));
                                    // FirePropertyChanged("ComponentProperty");
                                    if (propertychangedRaiser != null)
                                        c.Call(_proxy.This.Invoke(propertychangedRaiser, g.Const(pi.Name)));
                                }
                                c.End();
                            }
                        });
                // end of ComponentProperty
            }

            private void ImplProperty(PropertyInfo pi, Action<IMethod> getterImpl, Action<ISetter> setterImpl)
            {
                MethodInfo mGetter, mSetter;
                bool needsGetter = NeedsGetter(pi, out mGetter);
                bool needsSetter = NeedsSetter(pi, out mSetter);
                // if both getter and setter are implemented in base class, do nothing.
                if (!needsGetter && !needsSetter) return;
                var property = _proxy.Property(pi).Public;
                // if has getter and not implemented in base class.
                if (needsGetter)
                {
                    var getter = property.Getter();
                    if (mGetter != null) getter.Override(mGetter);
                    getterImpl(getter);
                }
                // if has setter and not implemented in base class.
                if (needsSetter)
                {
                    var setter = property.Setter();
                    if (mSetter != null) setter.Override(mGetter);
                    setterImpl(setter);
                }
            }

            private MethodInfo FindOnPropertyChangedMethod(PropertyInfo pi)
            {
                var attrs = pi.GetCustomAttributes(_raiserAttribute, false);
                // use the type level raise when no raiser attribute exists.
                if (attrs.Length == 0) return _propertyChangedRaiser;
                string methodName = _getPropertyChangedRaiserFromRaiserAttribute(attrs[0]);
                // don't raise event when raiser method name is null.
                if (methodName == null ) return null;
                MethodInfo result;
                // return from cache if one already exists.
                if (_propertyChangedRaiserCache.TryGetValue(methodName, out result)) return result;
                // try to find the specified method
                result = _baseType.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new[] {typeof (string)}, null);
                // error if no method found.
                if (result==null || !(result.IsPublic || result.IsFamily))
                {
                    throw new MissingMemberException(
                        string.Format("Missing method {2}.{3}(string) specified by attribute {1} on property {0}.",
                                      pi, attrs[0].GetType().FullName, _baseType, methodName));
                }
                // cache the raiser for future use.
                _propertyChangedRaiserCache[methodName] = result;
                return result;
            }

            private bool NeedsGetter(PropertyInfo interfaceProperty, out MethodInfo baseGetter)
            {
                baseGetter = null;
                if (!interfaceProperty.CanRead) return false; // property has no getter
                return NeedsMethod(interfaceProperty.GetGetMethod(), out baseGetter);
            }

            private bool NeedsSetter(PropertyInfo interfaceProperty, out MethodInfo baseSetter)
            {
                baseSetter = null;
                if (!interfaceProperty.CanWrite) return false; // property has no setter
                return NeedsMethod(interfaceProperty.GetSetMethod(), out baseSetter);
            }

            private bool NeedsMethod(MethodInfo interfaceMethod, out MethodInfo baseMethod)
            {
                baseMethod = FindMethodInBaseType(interfaceMethod);
                return baseMethod == null || baseMethod.IsAbstract;
            }

            private MethodInfo FindMethodInBaseType(MethodInfo interfaceMethod)
            {
                MethodInfo result;
                _interfaceToBaseMethods.TryGetValue(interfaceMethod, out result);
                return result;
            }

            private static void ProxyInvoke(ICode c, Type returnType, IParameterList args, 
                                            Func<FactoryPair, IOperand[], IOperand> callTarget)
            {
                bool hasRef = false;
                IOperand[] vars = new IOperand[args.Count];
                IOperand[] parameters = new IOperand[args.Count];
                FactoryPair[] pairs = new FactoryPair[args.Count];
                for (int i = 0; i < args.Count; i++)
                {
                    var arg = args[i];
                    var pair = GetFactoryPair(arg.Type);
                    if (pair == null) { parameters[i] = arg; continue; }
                    pairs[i] = pair;
                    if (arg.Direction == ParameterDirection.In)
                    {
                        parameters[i] = c.Invoke(pair.GetTarget, arg);
                        continue;
                    }
                    hasRef = true;
                    vars[i] = c.Variable(arg.Type.GetElementType(), arg.Name + "Target");
                    if (arg.Direction == ParameterDirection.Ref)
                    {
                        c.Assign(vars[i], c.Invoke(pair.GetTarget, arg));
                    }
                    parameters[i] = vars[i];
                }
                var returnPair = GetFactoryPair(returnType);
                var result = callTarget(returnPair, parameters);
                bool isVoid = returnType == typeof(void) || result == null;
                if (hasRef)
                {
                    if(!isVoid) result = c.Variable(returnType, "result", result);
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (vars[i] == null) continue;
                        c.Assign(args[i], c.Invoke(pairs[i].GetProxy, vars[i]));
                    }
                }
                if (result == null) c.Return();
                else if (returnPair == null) c.Return(result);
                else c.Return(c.Invoke(returnPair.GetProxy, result));
            }
        }

        private class FactoryPair
        {
            internal MethodInfo GetProxy;
            internal MethodInfo GetTarget;
        }
    }
}
