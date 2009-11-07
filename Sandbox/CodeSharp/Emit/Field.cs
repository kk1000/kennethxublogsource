using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Field definition.
    /// </summary>
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
    }
}