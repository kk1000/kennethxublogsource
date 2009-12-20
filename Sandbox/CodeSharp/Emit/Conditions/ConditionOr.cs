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

using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    /// <author>Kenneth Xu</author>
    internal class ConditionOr : Condition
    {
        private readonly Condition _condition1;
        private readonly Condition _condition2;

        public ConditionOr(Condition condition1, Condition condition2)
        {
            if (condition1 == null) throw new ArgumentNullException("condition1");
            if (condition2 == null) throw new ArgumentNullException("condition2");
            _condition1 = condition1;
            _condition2 = condition2;
        }

        internal override void EmitGet(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        public override void EmitBranch(ILGenerator il, Label label, bool reverse)
        {
            if (reverse) throw new NotImplementedException();
            var orLabel = il.DefineLabel();
            _condition1.EmitBranch(il, orLabel, true);
            _condition2.EmitBranch(il, label, false);
            il.MarkLabel(orLabel);
        }
    }
}
