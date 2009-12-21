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
    internal class Construction : Operand
    {
        private readonly IEnumerable<IOperand> _args;
        private readonly ConstructorInfo _constructorInfo;

        public Construction(Type type, IOperand[] args)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (args == null) args = EmptyOperands;
            _args = args;
            var paramTypes = new Type[args.Length];
            for (int i = args.Length - 1; i >= 0; i--)
            {
                paramTypes[i] = args[i].Type;
            }
            _constructorInfo = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, paramTypes, null);
            if (_constructorInfo == null)
            {
                StringBuilder sb = new StringBuilder("Not such constructor found: ");
                sb.Append(type).Append(".ctor").Append('(');
                foreach (var t in paramTypes)
                {
                    sb.Append(t).Append(',');
                }
                if (paramTypes.Length > 0)
                {
                    sb.Length -= 1;
                }
                sb.Append(')');
                throw new ArgumentException(sb.ToString());
            }
        }

        public override Type Type
        {
            get { return _constructorInfo.ReflectedType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            foreach (Operand operand in _args)
            {
                operand.EmitGet(il);
            }
            il.Emit(OpCodes.Newobj, _constructorInfo);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}
