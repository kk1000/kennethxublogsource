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
    internal class AssignmentCode : CodeSnip
    {
        private readonly Operand _target;
        private readonly Operand _value;

        public AssignmentCode(Operand target, Operand value)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (value == null) throw new ArgumentNullException("value");
            _target = target;
            _value = value;
        }

        public override void Emit(ILGenerator il)
        {
            _target.EmitSet(il, _value);
        }
    }
}
