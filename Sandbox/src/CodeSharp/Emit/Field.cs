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
    /// Field definition.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class Field : Operand, IField
    {
        private readonly string _name;
        private readonly Type _type;
        private FieldAttributes _fieldAttributes = FieldAttributes.Private | FieldAttributes.InitOnly;

        /// <summary>
        /// The <see cref="FieldBuilder"/> cooresponding to this field.
        /// </summary>
        public FieldBuilder FieldBuilder { get; private set; }

        /// <summary>
        /// Construct a new <see cref="Field"/> with given <paramref name="type"/>
        /// and <paramref name="name"/>.
        /// </summary>
        /// <param name="type">Type of the field.</param>
        /// <param name="name">Name of the field.</param>
        public Field(Type type, string name)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type == typeof(void)) throw new ArgumentException("Field type cannot be void.", "type");
            if(name==null) throw new ArgumentNullException("name");
            _type = type;
            _name = name;
        }

        /// <summary>
        /// Emit the IL code to define this field.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type that this field will be defined.
        /// </param>
        internal void Emit(TypeBuilder typeBuilder)
        {
            FieldBuilder = typeBuilder.DefineField(_name, _type, _fieldAttributes);
        }

        /// <summary>
        /// The type of the operand.
        /// </summary>
        public override Type Type
        {
            get { return _type; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            if (!FieldBuilder.IsStatic) il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, FieldBuilder);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            if (!FieldBuilder.IsStatic) il.Emit(OpCodes.Ldarg_0);
            value.EmitGet(il);
            il.Emit(OpCodes.Stfld, FieldBuilder);
        }

        public string Name
        {
            get { return _name; }
        }

        public IField ReadOnly
        {
            get
            {
                _fieldAttributes |= FieldAttributes.InitOnly;
                return this;
            }
        }

        public IField Internal
        {
            get
            {
                _fieldAttributes |= FieldAttributes.Assembly;
                return this;
            }
        }
    }
}
