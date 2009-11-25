using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    internal class ConditionOr : Condition
    {
        private readonly Condition _condition1;
        private readonly Condition _condition2;

        public ConditionOr(Condition condition1, Condition condition2)
        {
            if (condition1 == null) throw new ArgumentNullException("condition1");
            if (condition2 == null) throw new ArgumentNullException("condition2");
            _condition1 = condition1;
            _condition2 = condition2;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            if (reverse) throw new NotImplementedException();
            var orLabel = il.DefineLabel();
            _condition1.EmitBranch(il, orLabel, true);
            _condition2.EmitBranch(il, label, false);
            il.MarkLabel(orLabel);
        }
    }
}