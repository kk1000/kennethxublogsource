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
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit.Conditions
{
    /// <author>Kenneth Xu</author>
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
