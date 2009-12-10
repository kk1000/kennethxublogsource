using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
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