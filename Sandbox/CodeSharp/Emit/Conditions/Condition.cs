using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    internal abstract class Condition : Operand, ICondition
    {
        public abstract void EmitBranch(ILGenerator il, Label label, bool reverse);

        public override System.Type Type
        {
            get { return typeof(bool); }
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new InvalidOperationException();
        }
    }
}