using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class FieldAccess : Operand
    {
        private readonly Operand _operand;
        private readonly FieldInfo _fieldInfo;
        public FieldAccess(Operand operand, string name)
        {
            if (operand == null) throw new ArgumentNullException("operand");
            if (name == null) throw new ArgumentNullException("name");
            _operand = operand;
            _fieldInfo = operand.Type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override Type Type
        {
            get { return _fieldInfo.FieldType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            il.Emit(OpCodes.Ldfld, _fieldInfo);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}