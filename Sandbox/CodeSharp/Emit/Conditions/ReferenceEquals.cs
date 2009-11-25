using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    internal class ReferenceEquals : Condition
    {
        private readonly Operand _operand1;
        private readonly Operand _operand2;

        private static readonly MethodInfo _referenceEquals = 
            typeof (object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static);

        public ReferenceEquals(Operand operand1, Operand operand2)
        {
            if (operand1 == null) throw new ArgumentNullException("operand1");
            if (operand2 == null) throw new ArgumentNullException("operand2");
            _operand1 = operand1;
            _operand2 = operand2;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            _operand1.EmitGet(il);
            _operand2.EmitGet(il);
            il.Emit(OpCodes.Call, _referenceEquals);
            if(reverse) 
                il.Emit(OpCodes.Brtrue_S, label);
            else
                il.Emit(OpCodes.Brfalse_S, label);
        }
    }
}