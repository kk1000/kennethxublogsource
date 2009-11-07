using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSharp;

namespace CompareIL
{
    public class ReadOnlyPropertyImpl : IReadOnlyProperty
    {
        private readonly IReadOnlyProperty _wrapped;

        public ReadOnlyPropertyImpl(IReadOnlyProperty wrapped)
        {
            _wrapped = wrapped;
        }

        public int ReadOnlyProperty
        {
            get { return _wrapped.ReadOnlyProperty; }
        }
    }
}
