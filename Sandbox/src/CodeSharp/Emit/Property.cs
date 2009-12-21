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
    internal class Property : IProperty {
        private readonly Type _type;
        private readonly string _name;
        private readonly IParameter[] _parameters;
        private MethodAttributes _propertyAttributes;
        private Getter _getter;
        private Setter _setter;
        private PropertyBuilder _propertyBuilder;

        public Property(PropertyInfo propertyInfo)
            : this(propertyInfo.PropertyType, propertyInfo.Name, Parameter.From(propertyInfo.GetIndexParameters()))
        {
        }

        public Property(Type type, params IParameter[] parameters)
            : this(type, "Item", parameters)
        {
        }

        public Property(Type type, string name)
            : this(type, name, Parameter.EmptyParameters)
        {
        }

        private Property(Type type, string name, params IParameter[] parameters)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            if (type == typeof(void) || type.IsByRef)
            {
                throw new ArgumentException("Not a valid property type: " + type, "type");
            }
            _type = type;
            _name = name;
            _parameters = parameters;
            _propertyAttributes = 
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot | 
                MethodAttributes.Final | 
                MethodAttributes.SpecialName;
        }

        public void EmitDefinition(TypeBuilder typeBuilder)
        {
            _propertyBuilder = typeBuilder.DefineProperty(
                _name, PropertyAttributes.None, CallingConventions.HasThis, 
                _type, null, null, 
                ParameterList.ToTypes(_parameters), null, null);

            if (_getter != null)
            {
                _getter.EmitDefinition(typeBuilder);
                _propertyBuilder.SetGetMethod(_getter.MethodBuilder);
            }
            if (_setter != null)
            {
                _setter.EmitDefinition(typeBuilder);
                _propertyBuilder.SetSetMethod(_setter.MethodBuilder);
            }
        }

        public void EmitCode(TypeBuilder builder)
        {
            if (_getter != null) _getter.EmitCode();
            if (_setter != null) _setter.EmitCode();
        }

        public IProperty Public
        {
            get
            {
                _propertyAttributes |= MethodAttributes.Public;
                return this;
            }
        }

        public IProperty Override(PropertyInfo property)
        {
            var attrs = property.GetGetMethod(true).Attributes;
            _propertyAttributes = 
                _propertyAttributes & ~(MethodAttributes.MemberAccessMask|MethodAttributes.NewSlot) | 
                attrs & MethodAttributes.MemberAccessMask;
            return this;
        }

        public IMethod Getter()
        {
            _getter = new Getter(_type, "get_" + _name, _propertyAttributes, _parameters);
            return _getter;
        }

        public ISetter Setter()
        {
            _setter = new Setter(_type, "set_" + _name, _propertyAttributes, _parameters);
            return _setter;
        }
    }
}
