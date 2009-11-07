using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class PropertyAccess : Operand
    {
        private readonly Operand _operand;
        private readonly string _propertyName;
        private PropertyInfo _propertyInfo;

        public PropertyAccess(Operand operand, string name)
        {
            _operand = operand;
            _propertyName = name;
            _propertyInfo = operand.Type.GetProperty(name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (_propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Not such property found: {0}.{1}", operand.Type, name));
            }
        }

        public override Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetGetMethod(true));
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            _operand.EmitGet(il);
            value.EmitGet(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetSetMethod(true));
        }
    }
}