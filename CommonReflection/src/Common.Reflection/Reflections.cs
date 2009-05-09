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

namespace Common.Reflection
{
    /// <summary>
    /// Utility methods for common reflection tasks.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class Reflections
    {
        #region Constants
        private const BindingFlags ALL_STATIC_METHOD = 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod;

        private const BindingFlags ALL_INSTANCE_METHOD = 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
        #endregion

        #region Public Extension Methods

        public static TDelegate GetStaticMethod<TDelegate>(
            this Type type, string name)
        {
            return (TDelegate)GetStaticMethod<TDelegate>(type, name, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TDelegate GetStaticMethodOrFail<TDelegate>(this Type type, string name)
        {
            return (TDelegate)GetStaticMethod<TDelegate>(type, name, true);
        }

        public static TDelegate GetInstanceMethod<TDelegate>(this Type type, string name)
        {
            return (TDelegate)GetInstanceMethod<TDelegate>(type, name, false);
        }

        public static TDelegate GetInstanceMethodOrFail<TDelegate>(this Type type, string name)
        {
            return (TDelegate)GetInstanceMethod<TDelegate>(type, name, true);
        }

        public static TDelegate GetInstanceMethod<TDelegate>(this object obj, string name)
        {
            return (TDelegate)GetInstanceMethod<TDelegate>(obj, name, false);
        }

        public static TDelegate GetInstanceMethodOrFail<TDelegate>(this object obj, string name)
        {
            return (TDelegate)GetInstanceMethod<TDelegate>(obj, name, false);
        }

        public static TDelegate GetNonVirtualInvoker<TDelegate>(this Type type, string name)
        {
            return (TDelegate)GetNonVirtualInvoker<TDelegate>(type, name, false);
        }

        public static TDelegate GetNonVirtualInvokerOrFail<TDelegate>(this Type type, string name)
        {
            return (TDelegate)GetNonVirtualInvoker<TDelegate>(type, name, true);
        }

        public static TDelegate GetNonVirtualInvoker<TDelegate>(this object obj, Type type, string name)
        {
            return (TDelegate)GetNonVirtualInvoker<TDelegate>(obj, type, name, false);
        }

        public static TDelegate GetNonVirtualInvokerOrFail<TDelegate>(this object obj, Type type, string name)
        {
            return (TDelegate)GetNonVirtualInvoker<TDelegate>(obj, type, name, true);
        }

        #endregion

        #region Private Methods

        private static object GetStaticMethod<T>(Type type, string name, bool failFast)
        {
            MethodInfo method = GetMethodOrFail(typeof(T), type, name, ALL_STATIC_METHOD, false, failFast);
            return method==null ? null : Delegate.CreateDelegate(typeof(T), method);
        }

        private static object GetInstanceMethod<T>(Type type, string name, bool failFast)
        {
            MethodInfo method = GetMethodOrFail(typeof(T), type, name, ALL_INSTANCE_METHOD, true, failFast);
            return (method==null) ? null : Delegate.CreateDelegate(typeof(T), method);
        }

        private static object GetInstanceMethod<T>(object obj, string name, bool failFast)
        {
            MethodInfo method = GetMethodOrFail(typeof(T), obj.GetType(), name, ALL_INSTANCE_METHOD, false, failFast);
            return method==null ? null : Delegate.CreateDelegate(typeof(T), obj, method);
        }

        private static object GetNonVirtualInvoker<T>(Type type, string name, bool failFast)
        {
            MethodInfo method = GetMethodOrFail(typeof(T), type, name, ALL_INSTANCE_METHOD, true, failFast);
            if (method == null) return null;
            return method.IsVirtual ?
                CreateDynamicMethod(method).CreateDelegate(typeof(T)) :
                Delegate.CreateDelegate(typeof(T), method);
        }

        private static object GetNonVirtualInvoker<T>(object obj, Type type, string name, bool failFast)
        {
            MethodInfo method = GetMethodOrFail(typeof(T), type, name, ALL_INSTANCE_METHOD, false, failFast);
            if (method == null) return null;
            return method.IsVirtual ?
                CreateDynamicMethod(method).CreateDelegate(typeof(T), obj) :
                Delegate.CreateDelegate(typeof(T), obj, method);
        }

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

        private static MethodInfo GetMethodOrFail(Type delegateType, Type type, string name, BindingFlags bindingAttr, bool excludeFirst, bool failFast)
        {
            ParameterInfo[] parameters = GetDelegateParameters(delegateType);
            Type[] types = ExtractTypesFromParameters(parameters, excludeFirst);
            var method = type.GetMethod(name, bindingAttr, null, types, null);
            if (method == null && failFast)
            {
                throw new NoMatchException(type, bindingAttr, name, types);
            }
            return method;
        }

        private static Type[] ExtractTypesFromParameters(ParameterInfo[] parameters, bool excludeFirst)
        {
            int offset = excludeFirst ? 1 : 0;
            int size = parameters.Length - offset;

            Type[] types = new Type[size];
            for (int i = 0; i < size; i++)
            {
                types[i] = parameters[i+offset].ParameterType;
            }
            return types;
        }

        private static ParameterInfo[] GetDelegateParameters(Type delegateType)
        {
            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType))
            {
                throw new InvalidOperationException(
                    "Expecting type parameter to be a Delegate type, but got " +
                    delegateType.FullName);
            }
            var invoke = delegateType.GetMethod("Invoke");
            return invoke.GetParameters();
        }

        #endregion;
    }
}
