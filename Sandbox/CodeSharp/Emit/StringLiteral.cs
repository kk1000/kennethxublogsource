using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class StringLiteral : Operand
    {
        private readonly string _value;

        public StringLiteral(string value)
        {
            _value = value;
        }

        public override Type Type
        {
            get { return typeof (string); }
        }

        internal override void EmitGet(ILGenerator il)
        {
            il.Emit(OpCodes.Ldstr, _value);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new InvalidOperationException("Cannot assign to string literal");
        }
    }
}