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
    /// <author>Kenneth Xu</author>
    internal class FieldAccess : Operand
    {
        private readonly Operand _operand;
        private readonly Field _field;
        private readonly FieldInfo _fieldInfo;

        public FieldAccess(Operand operand)
        {
            if (operand == null) throw new ArgumentNullException("operand");
            _operand = operand;
        }
        public FieldAccess(Operand operand, string name) : this(operand)
        {
            if (name == null) throw new ArgumentNullException("name");
            _fieldInfo = operand.Type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public FieldAccess(Operand operand, IField field) : this (operand)
        {
            if (field == null) throw new ArgumentNullException("field");
            _field = (Field)field;
        }

        public override Type Type
        {
            get { return FieldInfo.FieldType; }
        }

        private FieldInfo FieldInfo
        {
            get {return (_fieldInfo??_field.FieldBuilder);}
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            il.Emit(OpCodes.Ldfld, FieldInfo);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}
