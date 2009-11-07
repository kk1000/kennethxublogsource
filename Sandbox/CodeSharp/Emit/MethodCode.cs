using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// To define the code in a method.
    /// </summary>
    class MethodCode : IMethodCode
    {
        List<Code> _codes = new List<Code>();
        /// <summary>
        /// Complete the code of the method and prevent further modification.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Return the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value to return.
        /// </param>
        public void Return(IOperand value)
        {
            _codes.Add(new ReturnCode((Operand)value));
        }

        /// <summary>
        /// Return.
        /// </summary>
        public void Return()
        {
            _codes.Add(new ReturnCode());
        }

        /// <summary>
        /// Assign the <paramref name="value"/> to <paramref name="target"/>
        /// </summary>
        /// <param name="target">
        /// Target to assign the value.
        /// </param>
        /// <param name="value">
        /// Value to be assigned.
        /// </param>
        public void Assign(IOperand target, IOperand value)
        {
            _codes.Add(new AssignmentCode((Operand) target, (Operand) value));
        }

        /// <summary>
        /// Emit IL code for method.
        /// </summary>
        /// <param name="il">
        /// The method to emit the code for.
        /// </param>
        public void Emit(ILGenerator il)
        {
            foreach (var code in _codes)
            {
                code.Emit(il);
            }
        }
    }

    internal class AssignmentCode : Code
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

    internal class ReturnCode : Code
    {
        private readonly Operand _operand;

        public ReturnCode(Operand operand)
        {
            _operand = operand;
        }

        public ReturnCode()
        {
        }

        public override void Emit(ILGenerator il)
        {
            if (_operand != null) _operand.EmitGet(il);
            il.Emit(OpCodes.Ret);
        }
    }

    internal abstract class Code
    {
        public abstract void Emit(ILGenerator il);
    }
}