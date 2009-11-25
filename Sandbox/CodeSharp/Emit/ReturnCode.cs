using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class ReturnCode : CodeSnip
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
}