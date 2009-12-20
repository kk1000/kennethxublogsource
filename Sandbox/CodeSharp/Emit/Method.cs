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
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Method definition.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class Method : Invokable<IMethod>, IMethod
    {
        protected MethodBuilder _methodBuilder;

        internal Method(MethodInfo methodInfo)
            : this(methodInfo.ReturnType, methodInfo.Name, Parameter.From(methodInfo.GetParameters()))
        {
        }

        internal Method(Type returnType, string name, ParameterList parameters)
            : base(returnType, name, parameters)
        {
            _methodAttributes = MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;
        }

        internal Method(Type returnType, string name, params IParameter[] parameters)
            : this(returnType, name, new ParameterList(parameters))
        {
        }

        internal Method(string name, ParameterList parameters)
            : this(typeof(void), name, parameters)
        {
        }

        /// <summary>
        /// Emit the IL code to define this method.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type that this field will be defined.
        /// </param>
        public override void EmitDefinition(TypeBuilder typeBuilder)
        {
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");
            var mb = typeBuilder.DefineMethod(_name, _methodAttributes, _returnType, _parameters.ToTypes());
            _methodBuilder = mb;
            _parameters.Emit(this);
        }

        protected override ILGenerator GetILGenerator()
        {
            return _methodBuilder.GetILGenerator();
        }

        public override ParameterBuilder DefineParameter(int i, ParameterAttributes attributes, string name)
        {
            if ((_methodAttributes & MethodAttributes.Static) == MethodAttributes.Static) i--;
            return _methodBuilder.DefineParameter(i, attributes, name);
        }

        protected override IMethod Self
        {
            get { return this; }
        }
    }
}
