using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class Getter : Method
    {
        public Getter(Type returnType, string name, MethodAttributes methodAttributes, params IParameter[] parameters) 
            : base(returnType, name, parameters)
        {
            _methodAttributes = methodAttributes;
        }

        public MethodBuilder MethodBuilder
        {
            get { return _methodBuilder; }
        }
    }
}