using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    internal class IsNull : Condition
    {
        private readonly Operand _operand;

        public IsNull(Operand operand)
        {
            if (operand == null) throw new ArgumentNullException("operand");
            _operand = operand;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            _operand.EmitGet(il);
            if (reverse) il.Emit(OpCodes.Brfalse_S, label);
            else il.Emit(OpCodes.Brtrue_S, label);
        }
    }
}