using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    internal class Equals : Condition
    {
        private readonly Operand _left;
        private readonly Operand _right;

        public Equals(Operand left, Operand right)
        {
            _left = left;
            _right = right;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new InvalidOperationException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            var type = _left.Type;
            MethodInfo equals = type.GetMethod(
                "op_Inequality",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] {type, type}, null);
            _left.EmitGet(il);
            _right.EmitGet(il);
            if (equals != null)
            {
                il.Emit(OpCodes.Call, equals);
                if(reverse) 
                    il.Emit(OpCodes.Brfalse_S, label);
                else
                    il.Emit(OpCodes.Brtrue_S, label);
            }
            else
            {
                if(reverse) 
                    il.Emit(OpCodes.Beq_S, label);
                else
                    throw new NotImplementedException();
            }
        }
    }
}