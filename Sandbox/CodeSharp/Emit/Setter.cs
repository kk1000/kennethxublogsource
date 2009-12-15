using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class Setter : Method, ISetter
    {
        private readonly ParameterList _origParameters;

        public Setter(Type type, string name, MethodAttributes methodAttributes, params IParameter[] parameters)
            : base(name, MakeSetterParameterList(type, parameters))
        {
            _origParameters = new ParameterList(parameters);
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

        IParameterList IInvokable.Args
        {
            get { return _origParameters; }
        }

        public IParameter Value
        {
            get { return Args[_origParameters.Count]; }
        }

        #region ISetter Members

        ISetter ISetter.Override(MethodInfo method)
        {
            Override(method);
            return this;
        }

        #endregion
    }
}