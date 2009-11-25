using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class AssignmentCode : CodeSnip
    {
        private readonly Operand _target;
        private readonly Operand _value;

        public AssignmentCode(Operand target, Operand value)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (value == null) throw new ArgumentNullException("value");
            _target = target;
            _value = value;
        }

        public override void Emit(ILGenerator il)
        {
            _target.EmitSet(il, _value);
        }
    }
}