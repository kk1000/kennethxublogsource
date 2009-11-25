using System.Reflection.Emit;
using CodeSharp.Emit.Conditions;

namespace CodeSharp.Emit
{
    internal class IfBlock : CodeBlock
    {
        private readonly Condition _condition;

        public IfBlock(Condition condition)
        {
            _condition = condition;
        }

        public override void Emit(ILGenerator il)
        {
            var label = il.DefineLabel();
            _condition.EmitBranch(il, label, false);
            foreach (var codeSnip in _codes)
            {
                codeSnip.Emit(il);
            }
            il.MarkLabel(label);
        }
    }
}