using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSharp;

namespace CompareIL
{
    public class WriteOnlyPropertyImpl : IWriteOnlyProperty
    {
        private readonly IWriteOnlyProperty _wrapped;

        public WriteOnlyPropertyImpl(IWriteOnlyProperty wrapped)
        {
            _wrapped = wrapped;
        }

        public int WriteOnlyProperty
        {
            set { _wrapped.WriteOnlyProperty = value; }
        }
    }
}
