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
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
    public interface IEmittedDelegate
    {
        Type Type { get; }
        MethodInfo InvokeMethod { get; }
        int Identifier { get; }
    }

    /// <author>Kenneth Xu</author>
    public static class EmitUtils
    {
        private static AssemblyBuilder _lastBuilder;

        public static ModuleBuilder CreateDynamicModule(string name)
        {
            AssemblyName an = new AssemblyName { Name = name };
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            _lastBuilder = ab;
            return ab.DefineDynamicModule(name, name);
        }

        internal static void SaveAssembly(string name)
        {
            _lastBuilder.Save(name);
        }

        #region GenerateDelegateType and related
        private class EmittedDelegate : IEmittedDelegate
        {
            public Type Type { get; internal set; }
            public MethodInfo InvokeMethod { get; internal set; }
            public int Identifier { get; internal set; }
        }

        private static int DelegateCounter;

        private const MethodAttributes DelegateConstructorAttributes =
            MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;

        private const MethodAttributes DelegateInvokeMethodAttributes =
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

        private const TypeAttributes DelegateTypeAttributes =
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed |
            TypeAttributes.AnsiClass | TypeAttributes.AutoClass;

        private const MethodImplAttributes DelegateMemberImplemenationFlags =
            MethodImplAttributes.Runtime | MethodImplAttributes.Managed;

        private static readonly Type[] DelegateConstructorTypes =
            new Type[] { typeof(object), typeof(IntPtr) };

        public static IEmittedDelegate GenerateDelegateType(this ModuleBuilder modBuilder, MethodInfo targetMethod, bool isInstanceToStatic)
        {
            // Create a delegate that has the same signature as the method we would like to hook up to
            int identifier = Interlocked.Increment(ref DelegateCounter);

            string delegateName = modBuilder.Assembly.GetName().Name + ".Delegates.D" + identifier;

            var typeBuilder = modBuilder.DefineType(delegateName, DelegateTypeAttributes, typeof(MulticastDelegate));

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                DelegateConstructorAttributes, CallingConventions.Standard, DelegateConstructorTypes);

            constructorBuilder.SetImplementationFlags(DelegateMemberImplemenationFlags);

            // Grab the parameters of the method
            Type[] paramTypes = GetParamTypes(targetMethod, isInstanceToStatic);

            // Define the Invoke method for the delegate
            var methodBuilder = typeBuilder.DefineMethod(
                "Invoke", DelegateInvokeMethodAttributes, targetMethod.ReturnType, paramTypes);

            methodBuilder.SetImplementationFlags(DelegateMemberImplemenationFlags);

            // bake it!
            Type t = typeBuilder.CreateType();
            return new EmittedDelegate { Type = t, InvokeMethod = methodBuilder, Identifier = identifier };
        }
        #endregion

        #region GetParamTypes from MethodInfo and related
        public static Type[] GetParamTypes(this MethodInfo targetMethod)
        {
            return GetParamTypes(targetMethod, false);
        }

        public static Type[] GetInstanceToStaticParamTypes(this MethodInfo targetMethod)
        {
            return GetParamTypes(targetMethod, true);
        }

        private static Type[] GetParamTypes(MethodInfo targetMethod, bool isInstanceToStatic)
        {
            ParameterInfo[] parameters = targetMethod.GetParameters();
            Type[] paramTypes;
            int offset = 0;
            if (isInstanceToStatic)
            {
                paramTypes = new Type[parameters.Length + 1];
                paramTypes[0] = targetMethod.DeclaringType;
                offset = 1;
            }
            else
            {
                paramTypes = new Type[parameters.Length];
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                paramTypes[i + offset] = parameters[i].ParameterType;
            }
            return paramTypes;
        }
        #endregion
    }
}
