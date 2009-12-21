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

namespace CodeSharp.Emit
{
    /// <summary>
    /// Definition of parameter of method, delegate, indexer and etc.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class Parameter : Operand, IParameter
    {
        private readonly Type _type;
        private readonly string _name;
        private readonly ParameterDirection _direction;
        private ParameterBuilder _parameterBuilder;
        public static readonly IParameter[] EmptyParameters = new IParameter[0];

        /// <summary>
        /// Gets the type of parameter.
        /// </summary>
        public override Type Type
        {
            get
            {
                bool isDef = _direction == ParameterDirection.Out || _direction == ParameterDirection.Ref;
                return isDef ? _type.MakeByRefType() : _type;
            }
        }

        /// <summary>
        /// Construct a new instance of <see cref="Parameter"/>
        /// </summary>
        /// <param name="type">
        /// Type of the parameter.
        /// </param>
        /// <param name="name">
        /// Name of the parameter.
        /// </param>
        /// <param name="direction">
        /// The direction of the parameter.
        /// </param>
        public Parameter(Type type, string name, ParameterDirection direction)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            //TODO_Future if (type.IsByRef) type = type.GetElementType();
            _type = type;
            _name = name;
            _direction = direction;
        }

        public Parameter(ParameterInfo parameterInfo)
        {
            _direction = DirectionOf(parameterInfo);
            var type = parameterInfo.ParameterType;
            _type = type.IsByRef ? type.GetElementType() : type;
            _name = parameterInfo.Name;
        }

        /// <summary>
        /// Defines the parameter.
        /// </summary>
        /// <param name="i">
        /// Position of parameter.
        /// </param>
        /// <param name="invokable">
        /// The method that this parameter is defined. 
        /// </param>
        internal void Emit(int i, Invokable invokable)
        {
            ParameterAttributes attributes = _direction == ParameterDirection.Out ? ParameterAttributes.Out : 0;
            _parameterBuilder = invokable.DefineParameter(i, attributes, _name);
        }

        internal override void EmitGet(ILGenerator il)
        {
            ushort position = (ushort)_parameterBuilder.Position;
            switch (position)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldarg_S, position);
                    break;
            }
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            EmitGet(il);
            value.EmitGet(il);
            il.Emit(OpCodes.Stind_Ref);
        }

        internal static IParameter[] From(ParameterInfo[] parameterInfos)
        {
            if (parameterInfos == null) return null;
            var parameters = new IParameter[parameterInfos.Length];
            for (int i = parameterInfos.Length - 1; i >= 0; i--)
            {
                parameters[i] = new Parameter(parameterInfos[i]);
            }
            return parameters;
        }

        internal static ParameterDirection DirectionOf(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            return parameterInfo.ParameterType.IsByRef
                    ? (parameterInfo.IsOut ? ParameterDirection.Out : ParameterDirection.Ref)
                    : ParameterDirection.In;
        }

        public ParameterDirection Direction
        {
            get { return _direction; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
