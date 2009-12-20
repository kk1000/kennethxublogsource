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
using System.Collections.Generic;

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
    internal abstract class CodeBlock : CodeSnip
    {
        protected List<CodeSnip> _codes = new List<CodeSnip>();
        protected CodeBlock _nestedblock;

        public virtual void AddCodeSnip(CodeSnip snip)
        {
            if (_nestedblock == null)
            {
                _codes.Add(snip);
            }
            else
            {
                _nestedblock.AddCodeSnip(snip);
            }
        }

        public virtual void StartBlock(CodeBlock block)
        {
            if (_nestedblock == null)
            {
                _nestedblock = block;
                _codes.Add(block);
            }
            else
            {
                _nestedblock.StartBlock(block);
            }
        }

        public virtual void EndBlock()
        {
            if (_nestedblock == null) throw new InvalidOperationException();
            if (_nestedblock._nestedblock == null)
            {
                _nestedblock = null;
            }
            else
            {
                _nestedblock.EndBlock();
            }
        }
    }
}
