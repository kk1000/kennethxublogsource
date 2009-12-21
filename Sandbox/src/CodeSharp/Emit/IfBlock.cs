#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System.Collections.Generic;
using System.Reflection.Emit;
using CodeSharp.Emit.Conditions;

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
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
