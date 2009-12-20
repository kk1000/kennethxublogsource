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
    /// <author>Kenneth Xu</author>
    class Constructor : Invokable<IConstructor>, IConstructor
    {
        protected ConstructorBuilder _constructorBuilder;

        internal Constructor(params IParameter[] parameters)
            :base(null, ".ctor", new ParameterList(parameters))
        {
            _methodAttributes = MethodAttributes.HideBySig;
        }

        public override void EmitDefinition(TypeBuilder typeBuilder)
        {
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");
            var mb = typeBuilder.DefineConstructor(_methodAttributes, CallingConventions.HasThis, _parameters.ToTypes());
            _constructorBuilder = mb;
            _parameters.Emit(this);
        }

        protected override ILGenerator GetILGenerator()
        {
            return _constructorBuilder.GetILGenerator();
        }

        public override ParameterBuilder DefineParameter(int i, ParameterAttributes attributes, string name)
        {
            return _constructorBuilder.DefineParameter(i, attributes, name);
        }

        protected override IConstructor Self
        {
            get { return this; }
        }

        public override void EmitCode()
        {
            var type = _constructorBuilder.DeclaringType.BaseType;
            var mi = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var il = GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mi);
            base.EmitCode();
        }
    }
}
