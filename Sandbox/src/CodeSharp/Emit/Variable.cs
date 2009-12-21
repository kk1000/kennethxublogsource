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
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
    internal class Variable : Operand
    {
        private readonly Type _type;
        private readonly Class _class;
        private readonly string _name;
        private LocalBuilder _variable;

        public Variable(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        public Variable(IClass @class, string name)
        {
            _class = (Class) @class;
            _name = name;
        }

        public override Type Type
        {
            get
            {
                return _type??_class.TypeBuilder;
            }
        }

        internal override void EmitGet(ILGenerator il)
        {
            ushort position = (ushort) _variable.LocalIndex;
            switch (position)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldarg_S, position);
                    break;
            }
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            value.EmitGet(il);
            ushort position = (ushort)_variable.LocalIndex;
            switch (position)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    il.Emit(OpCodes.Stloc_S, position);
                    break;
            }
        }

        internal override void EmitByRef(ILGenerator il)
        {
            il.Emit(OpCodes.Ldloca_S, _variable);
        }

        internal void EmitDefinition(ILGenerator il)
        {
            _variable = il.DeclareLocal(Type);
        }
    }
}
