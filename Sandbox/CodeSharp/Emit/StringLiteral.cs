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

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
    internal class StringLiteral : Operand
    {
        private readonly string _value;

        public StringLiteral(string value)
        {
            _value = value;
        }

        public override Type Type
        {
            get { return typeof (string); }
        }

        internal override void EmitGet(ILGenerator il)
        {
            il.Emit(OpCodes.Ldstr, _value);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new InvalidOperationException("Cannot assign to string literal");
        }
    }
}
