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
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <author>Kenneth Xu</author>
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
