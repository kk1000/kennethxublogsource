using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    class ConditionNot : Condition
    {
        private readonly Condition _condition;

        public ConditionNot(Condition condition)
        {
            if (condition == null) throw new ArgumentNullException("condition");
            _condition = condition;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            _condition.EmitBranch(il, label, !reverse);
        }
    }
}
