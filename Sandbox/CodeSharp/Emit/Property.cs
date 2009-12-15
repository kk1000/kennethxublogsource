using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
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