using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class Setter : Method, ISetter
    {
        public Setter(Type type, string name, MethodAttributes methodAttributes, params IParameter[] parameters)
            : base(name, MakeSetterParameterList(type, parameters))
        {
            _methodAttributes |= methodAttributes | MethodAttributes.SpecialName;
        }

        private static ParameterList MakeSetterParameterList(Type propertyType, params IParameter[] parameters)
        {
            List<IParameter> setterParams = new List<IParameter>(parameters.Length + 1);
            setterParams.AddRange(parameters);
            setterParams.Add(new Parameter(propertyType, "value", ParameterDirection.In));
            return new ParameterList(setterParams);
        }

        public MethodBuilder MethodBuilder
        {
            get { return _methodBuilder; }
        }

        public IParameter Value
        {
            get { return Arg[Arg.Count-1]; }
        }
    }
}