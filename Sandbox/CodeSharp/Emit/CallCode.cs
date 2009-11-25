using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class CallCode : CodeSnip
    {
        private readonly Operand _statement;

        public CallCode(Operand statement)
        {
            _statement = statement;
        }

        public override void Emit(ILGenerator il)
        {
            _statement.EmitGet(il);
        }
    }
}