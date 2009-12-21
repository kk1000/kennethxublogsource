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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Represents method invoation.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class Invocation : Operand
    {
        private readonly Operand _operand;
        private readonly IEnumerable<IOperand> _args;
        private readonly MethodInfo _methodInfo;

        public Invocation(IOperand operand, MethodInfo methodInfo, IEnumerable<IOperand> args)
            :this(operand, args)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            _methodInfo = methodInfo;
            if (!methodInfo.IsStatic) GetOperandType(operand);
        }

        public Invocation(MethodInfo methodInfo, IEnumerable<IOperand> args)
            : this(null, methodInfo, args)
        {
        }

        /// <summary>
        /// Construct a new instance of <see cref="Invocation"/> that invokes
        /// method of given <paramref name="name"/> on given <paramref name="operand"/>
        /// with given <paramref name="args">parameters</paramref>.
        /// </summary>
        /// <param name="operand">
        /// The target to invoke the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The paremeter used to invoke the method.
        /// </param>
        public Invocation(IOperand operand, string name, IOperand[] args)
            :this(operand, GetOperandType(operand), name, args)
        {
        }

        public Invocation(Type type, string name, IOperand[] args)
            : this(null, type, name, args)
        {
        }

        private Invocation(IOperand operand, IEnumerable<IOperand> args)
        {
            _operand = (Operand)operand;
            _args = args ?? EmptyOperands;
        }

        private Invocation(IOperand operand, Type type, string name, IOperand[] args)
            : this(operand, args)
        {
            var flag = (operand == null ? BindingFlags.Static : BindingFlags.Instance);
            _methodInfo = GetMethodInfo(type, name, args, flag);
        }

        private static Type GetOperandType(IOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand", "operand is required to invoke instance method.");
            }
            return operand.Type;
        }

        private static MethodInfo GetMethodInfo(Type type, string name, IOperand[] args, BindingFlags flag)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            if (args == null) args = EmptyOperands;
            var paramTypes = new Type[args.Length];
            for (int i = args.Length - 1; i >= 0; i--)
            {
                paramTypes[i] = args[i].Type;
            }
            var methodInfo = type.GetMethod(
                name, BindingFlags.Public | BindingFlags.NonPublic | flag,
                null, paramTypes, null);
            if (methodInfo == null)
            {
                StringBuilder sb = new StringBuilder("Not such method found: ");
                sb.Append(type).Append('.').Append(name).Append('(');
                foreach (var paramType in paramTypes)
                {
                    sb.Append(paramType).Append(',');
                }
                if (paramTypes.Length > 0)
                {
                    sb.Length -= 1;
                }
                sb.Append(')');
                throw new ArgumentException(sb.ToString());
            }
            return methodInfo;
        }

        /// <summary>
        /// The type of the operand.
        /// </summary>
        public override Type Type
        {
            get { return _methodInfo.ReturnType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            Emit(il);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }

        internal void Emit(ILGenerator il)
        {
            var isInstance = !_methodInfo.IsStatic;
            if (isInstance) _operand.EmitGet(il);
            int i = 0;
            var parameters = _methodInfo.GetParameters();
            foreach (var operand in _args)
            {
                var type = parameters[i++].ParameterType;
                if (type.IsByRef && !operand.Type.IsByRef)
                {
                    ((Operand)operand).EmitByRef(il);
                    continue;
                }
                ((Operand)operand).EmitGet(il);
                if (!type.IsByRef && operand.Type.IsByRef)
                {
                    il.Emit(OpCodes.Ldind_Ref);
                }
            }
            if(isInstance)il.Emit(OpCodes.Callvirt, _methodInfo);
            else il.Emit(OpCodes.Call, _methodInfo);
        }
    }
}
