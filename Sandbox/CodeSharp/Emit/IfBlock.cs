using System.Collections.Generic;
using System.Reflection.Emit;
using CodeSharp.Emit.Conditions;

namespace CodeSharp.Emit
{
    internal class IfBlock : CodeBlock
    {
        private readonly Condition _condition;
        private readonly List<CodeSnip> _ifBlockCode;
        private bool _hasElse;

        public IfBlock(Condition condition)
        {
            _condition = condition;
            _ifBlockCode = _codes;
        }

        internal void StartElse()
        {
            _codes = new List<CodeSnip>();
            _hasElse = true;
        }

        public override void Emit(ILGenerator il)
        {
            var label = il.DefineLabel();
            _condition.EmitBranch(il, label, false);
            foreach (var codeSnip in _ifBlockCode)
            {
                codeSnip.Emit(il);
            }
            if (_hasElse)
            {
                var elseLabel = label;
                label = il.DefineLabel();
                il.Emit(OpCodes.Br_S, elseLabel);
                il.MarkLabel(elseLabel);
                foreach (var codeSnip in _codes)
                {
                    codeSnip.Emit(il);
                }
            }
            il.MarkLabel(label);
        }
    }
}