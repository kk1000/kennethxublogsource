#region License

/*
 * Copyright (C) 2009 the original author or authors.
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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Common.Reflection
{
    /// <summary>
    /// Utility methods for common reflection tasks.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class Reflections
    {
        #region Public Extension Methods

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// static method with given method <paramref name="name"/> from given
        /// <paramref name="type"/>. The method signature must be compatible
        /// the parameter and return type of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to locate the compatible method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <seealso cref="GetStaticInvokerOrFail{TDelegate}"/>
        public static TDelegate GetStaticInvoker<TDelegate>(this Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(type, name, false, false).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// static method with given method <paramref name="name"/> from given
        /// <paramref name="type"/>. The method signature must be compatible with 
        /// the parameter and return type of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/>.
        /// </returns>
        /// <exception name="NoMatchException">
        /// When there is no matching method found.
        /// </exception>
        /// <seealso cref="GetStaticInvoker{TDelegate}"/>
        public static TDelegate GetStaticInvokerOrFail<TDelegate>(this Type type, string name)
            where TDelegate : class 
        {
            return new DelegateBuilder<TDelegate>(type, name, true, false).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// instance method with given method <paramref name="name"/> from given
        /// <paramref name="type"/>. The first parameter type of <c>TDelegate</c> 
        /// must be assignable to the given <paramref name="type"/>. The rest
        /// parameters and return type of <c>TDelegate</c> must be compatible with 
        /// the signature of the method.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <seealso cref="GetInstanceInvokerOrFail{TDelegate}(System.Type,string)"/>
        /// <seealso cref="GetInstanceInvoker{TDelegate}(object,string)"/>
        public static TDelegate GetInstanceInvoker<TDelegate>(this Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(type, name, false, true).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// instance method with given method <paramref name="name"/> from given
        /// <paramref name="type"/>. The first parameter type of <c>TDelegate</c> 
        /// must be assignable to the given <paramref name="type"/>. The rest
        /// parameters and return type of <c>TDelegate</c> must be compatible with 
        /// the signature of the method.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <exception name="NoMatchException">
        /// When there is no matching method found.
        /// </exception>
        /// <seealso cref="GetInstanceInvoker{TDelegate}(System.Type,string)"/>
        /// <seealso cref="GetInstanceInvokerOrFail{TDelegate}(object,string)"/>
        public static TDelegate GetInstanceInvokerOrFail<TDelegate>(this Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(type, name, true, true).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// instance method with given method <paramref name="name"/> on specific
        /// <paramref name="obj">object</paramref>. The method signature must be 
        /// compatible with the signature of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="obj">
        /// The object instance to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <seealso cref="GetInstanceInvokerOrFail{TDelegate}(object,string)"/>
        /// <seealso cref="GetInstanceInvoker{TDelegate}(System.Type,string)"/>
        public static TDelegate GetInstanceInvoker<TDelegate>(this object obj, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(obj, obj.GetType(), name, false).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type 
        /// <typeparamref name="TDelegate"/> that can be used to call the 
        /// instance method with given method <paramref name="name"/> on specific
        /// <paramref name="obj">object</paramref>. The method signature must be 
        /// compatible with the signature of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="obj">
        /// The object instance to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <exception name="NoMatchException">
        /// When there is no matching method found.
        /// </exception>
        /// <seealso cref="GetInstanceInvokerOrFail{TDelegate}(System.Type,string)"/>
        /// <seealso cref="GetInstanceInvoker{TDelegate}(object ,string)"/>
        public static TDelegate GetInstanceInvokerOrFail<TDelegate>(this object obj, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(obj, obj.GetType(), name, true).CreateInvoker();
        }

        /// <summary>
        /// Extension method to obtain a delegate of type specified by parameter
        /// <typeparamref name="TDelegate"/> that can be used to make non virtual
        /// call to instance method with given method <paramref name="name"/> on 
        /// given <paramref name="type"/>. The first parameter type of <c>TDelegate</c> 
        /// must be assignable to the given <paramref name="type"/>. The rest
        /// parameters and return type of <c>TDelegate</c> must be compatible with 
        /// the signature of the method.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <seealso cref="GetNonVirtualInvokerOrFail{TDelegate}(Type,string)"/>
        /// <seealso cref="GetNonVirtualInvoker{TDelegate}(object,Type,string)"/>
        public static TDelegate GetNonVirtualInvoker<TDelegate>(this Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(type, name, false, true).CreateInvoker(true);
        }

        /// <summary>
        /// Extension method to obtain a delegate of type specified by parameter
        /// <typeparamref name="TDelegate"/> that can be used to make non virtual
        /// call to instance method with given method <paramref name="name"/> on 
        /// given <paramref name="type"/>. The first parameter type of <c>TDelegate</c> 
        /// must be assignable to the given <paramref name="type"/>. The rest
        /// parameters and return type of <c>TDelegate</c> must be compatible with 
        /// the signature of the method.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <exception name="NoMatchException">
        /// When there is no matching method found.
        /// </exception>
        /// <seealso cref="GetNonVirtualInvoker{TDelegate}(Type,string)"/>
        /// <seealso cref="GetNonVirtualInvokerOrFail{TDelegate}(object,Type,string)"/>
        public static TDelegate GetNonVirtualInvokerOrFail<TDelegate>(this Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(type, name, true, true).CreateInvoker(true);
        }

        /// <summary>
        /// Extension method to obtain a delegate of type specified by parameter
        /// <typeparamref name="TDelegate"/> that can be used to make non virtual
        /// call on the specific <paramref name="obj">object</paramref> to the
        /// instance method of given <paramref name="name"/> defined in the given 
        /// <paramref name="type"/> or its ancestor. The method signature must be 
        /// compatible with the signature of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="obj">
        /// The object instance to invoke the method.
        /// </param>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <seealso cref="GetNonVirtualInvokerOrFail{TDelegate}(object,Type,string)"/>
        /// <seealso cref="GetNonVirtualInvoker{TDelegate}(System.Type,string)"/>
        public static TDelegate GetNonVirtualInvoker<TDelegate>(this object obj, Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(obj, type, name, false).CreateInvoker(true);
        }

        /// <summary>
        /// Extension method to obtain a delegate of type specified by parameter
        /// <typeparamref name="TDelegate"/> that can be used to make non virtual
        /// call on the specific <paramref name="obj">object</paramref> to the
        /// instance method of given <paramref name="name"/> defined in the given 
        /// <paramref name="type"/> or its ancestor. The method signature must be 
        /// compatible with the signature of <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// Type of a .Net delegate.
        /// </typeparam>
        /// <param name="obj">
        /// The object instance to invoke the method.
        /// </param>
        /// <param name="type">
        /// The type to find the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// A delegate of type <typeparamref name="TDelegate"/> or null when
        /// no matching method if found.
        /// </returns>
        /// <exception name="NoMatchException">
        /// When there is no matching method found.
        /// </exception>
        /// <seealso cref="GetNonVirtualInvokerOrFail{TDelegate}(Type,string)"/>
        /// <seealso cref="GetNonVirtualInvoker{TDelegate}(object,Type,string)"/>
        public static TDelegate GetNonVirtualInvokerOrFail<TDelegate>(this object obj, Type type, string name)
            where TDelegate : class
        {
            return new DelegateBuilder<TDelegate>(obj, type, name, true).CreateInvoker(true);
        }

        #endregion

        #region Internal Methods

        internal static DynamicMethod CreateDynamicMethod(MethodInfo method)
        {
            int offset = (method.IsStatic ? 0 : 1);
            var parameters = method.GetParameters();
            int size = parameters.Length + offset;
            Type[] types = new Type[size];
            if (offset > 0) types[0] = method.DeclaringType;
            for (int i = offset; i < size; i++)
            {
                types[i] = parameters[i - offset].ParameterType;
            }

            DynamicMethod dynamicMethod = new DynamicMethod(
                "NonVirtualInvoker_" + method.Name, method.ReturnType, types, method.DeclaringType);
            ILGenerator il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < types.Length; i++) il.Emit(OpCodes.Ldarg, i);
            il.EmitCall(OpCodes.Call, method, null);
            il.Emit(OpCodes.Ret);
            return dynamicMethod;
        }

        internal static void AssertIsDelegate(Type delegateType)
        {
            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType))
            {
                throw new InvalidOperationException(
                    "Expecting type parameter to be a Delegate type, but got " +
                    delegateType.FullName);
            }
        }

        #endregion;

        private class DelegateBuilder<T> where T : class
        {
            #region Constants
            private const BindingFlags ALL_STATIC_METHOD =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod;

            private const BindingFlags ALL_INSTANCE_METHOD =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            #endregion

            private readonly bool _failFast;
            private readonly string _methodName;
            private readonly Type _targetType;
            private readonly object _targetObject;
            private readonly BindingFlags _bindingAttr;
            private Type _returnType;
            private Type[] _parameterTypes;

            public DelegateBuilder(object targetObject, Type targetType, string methodName, bool failFast)
                :this(targetObject, targetType, methodName, failFast, ALL_INSTANCE_METHOD)
            {
            }

            public DelegateBuilder(Type targetType, string methodName, bool failFast, bool isInstanceMethod)
                : this(null, targetType, methodName, failFast, isInstanceMethod ? ALL_INSTANCE_METHOD : ALL_STATIC_METHOD)
            {
            }

            private DelegateBuilder(object targetObject, Type targetType, string methodName, bool failFast, BindingFlags bindingAttr)
            {
                AssertIsDelegate(typeof(T));

                _targetObject = targetObject;
                _targetType = targetType;
                _methodName = methodName;
                _failFast = failFast;
                _bindingAttr = bindingAttr;
            }

            public T CreateInvoker()
            {
                return CreateInvoker(false);
            }

            public T CreateInvoker(bool nonVirtual)
            {
                var method = GetMethod();
                if (method == null) return null;
                try
                {
                    if (nonVirtual && method.IsVirtual)
                    {
                        var dynamicMethod = CreateDynamicMethod(method);
                        return _targetObject == null ?
                            dynamicMethod.CreateDelegate(typeof(T)) as T :
                            dynamicMethod.CreateDelegate(typeof(T), _targetObject) as T;
                    }
                    return _targetObject == null ? 
                        Delegate.CreateDelegate(typeof(T), method) as T :
                        Delegate.CreateDelegate(typeof(T), _targetObject, method) as T;
                }
                catch (ArgumentException ex)
                {
                    if (!_failFast) return null;
                    throw new NoMatchException(BuildExceptionMessage(), ex);
                }
            }

            private MethodInfo GetMethod()
            {
                MethodInfo invokeMethod = typeof(T).GetMethod("Invoke");
                ParameterInfo[] parameters = invokeMethod.GetParameters();
                _returnType = invokeMethod.ReturnType;
                bool instanceToStatic = (_targetObject == null && _bindingAttr == ALL_INSTANCE_METHOD);
                if (instanceToStatic)
                {
                    if (parameters.Length == 0)
                    {
                        throw new InvalidOperationException(string.Format(
                            "Delegate {0} has no parameter. It is required to have at least one parameter that is assignable from target type.",
                            typeof(T)));
                    }
                    Type instanceType = parameters[0].ParameterType;
                    if (!_targetType.IsAssignableFrom(instanceType))
                    {
                        if (!_failFast) return null;
                        throw new NoMatchException(string.Format(
                            "Target type {0} is not assignable to the first parameter of delegate {1}.",
                            _targetType, instanceType));
                    }
                }

                int offset = instanceToStatic ? 1 : 0;
                int size = parameters.Length - offset;

                Type[] types = new Type[size];
                for (int i = 0; i < size; i++)
                {
                    types[i] = parameters[i + offset].ParameterType;
                }
                _parameterTypes = types;

                var method = _targetType.GetMethod(_methodName, _bindingAttr, null, _parameterTypes, null);
                if (method == null && _failFast)
                {
                    throw new NoMatchException(BuildExceptionMessage());
                }
                return method;
            }

            private string BuildExceptionMessage()
            {
                StringBuilder sb = new StringBuilder()
                    .Append("No matching method found in the type ")
                    .Append(_targetType)
                    .Append(" for signature ")
                    .Append(_returnType).Append(" ")
                    .Append(_methodName).Append("(");
                if (_parameterTypes.Length > 0)
                {
                    foreach (Type parameter in _parameterTypes)
                    {
                        sb.Append(parameter).Append(", ");
                    }
                    sb.Length -= 2;
                }
                sb.Append(") with binding flags: ").Append(_bindingAttr).Append(".");
                return sb.ToString();
            }
        }
    }
}
