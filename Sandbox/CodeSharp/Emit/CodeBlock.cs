using System;
using System.Collections.Generic;

namespace CodeSharp.Emit
{
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