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
    /// <author>Kenneth Xu</author>
    internal class PropertyAccess : Operand
    {
        private readonly Operand _operand;
        private readonly IEnumerable<IOperand> _args;
        private readonly PropertyInfo _propertyInfo;

        public PropertyAccess(Operand operand, PropertyInfo propertyInfo, IEnumerable<IOperand> indexes)
        {
            _operand = operand;
            _args = indexes ?? EmptyOperands;
            _propertyInfo = propertyInfo;
        }

        public PropertyAccess(Operand operand, string name)
            : this(operand, GetPropertyInfo(operand.Type, name, null), null)
        {
        }

        public PropertyAccess(Operand operand, params IOperand[] indexes)
            : this(operand, GetPropertyInfo(operand.Type, "Item", indexes), indexes)
        {
        }

        private static PropertyInfo GetPropertyInfo(Type t, string name, IList<IOperand> indexes)
        {
            var pi = t.GetProperty(
                name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, null, ToTypes(indexes)??Type.EmptyTypes, null);
            if (pi == null)
            {
                if(indexes == null || indexes.Count == 0)
                {
                    throw new ArgumentException(string.Format("Not such property found: {0}.{1}", t, name));
                }
                StringBuilder sb = new StringBuilder("Not such indexer found: ");
                sb.Append(t).Append('[');
                foreach (var arg in indexes)
                {
                    sb.Append(arg.Type).Append(',');
                }
                if (indexes.Count > 0)
                {
                    sb.Length -= 1;
                }
                sb.Append(']');
                throw new ArgumentException(sb.ToString());
            }
            return pi;
        }

        public override Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            EmitArgs(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetGetMethod(true));
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            _operand.EmitGet(il);
            EmitArgs(il);
            value.EmitGet(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetSetMethod(true));
        }

        private void EmitArgs(ILGenerator il)
        {
            foreach (var operand in _args)
            {
                ((Operand)operand).EmitGet(il);
            }
        }
    }
}
