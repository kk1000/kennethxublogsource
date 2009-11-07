using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class Setter : Method, ISetter
    {
        public Setter(Type type, string name, MethodAttributes methodAttributes)
            : base(name, new Parameter(type, "value", ParameterDirection.In))
        {
            _methodAttributes |= methodAttributes | MethodAttributes.SpecialName;
        }

        public MethodBuilder MethodBuilder
        {
            get { return _methodBuilder; }
        }

        public IParameter Value
        {
            get { return Arg[0]; }
        }
    }
}