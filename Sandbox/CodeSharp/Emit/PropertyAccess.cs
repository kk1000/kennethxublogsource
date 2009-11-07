using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CodeSharp.Emit
{
    internal class PropertyAccess : Operand
    {
        private readonly Operand _operand;
        private readonly string _propertyName;
        private readonly IOperand[] _args;
        private PropertyInfo _propertyInfo;

        public PropertyAccess(Operand operand, string name)
            : this(operand, name, null)
        {
            //_operand = operand;
            //_propertyName = name;
            //_propertyInfo = operand.Type.GetProperty(name, 
            //    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //if (_propertyInfo == null)
            //{
            //    throw new ArgumentException(string.Format("Not such property found: {0}.{1}", operand.Type, name));
            //}
        }

        public PropertyAccess(Operand operand, params IOperand[] args)
            : this(operand, "Item", args)
        {
        }

        private PropertyAccess(Operand operand, string name, params IOperand[] args)
        {
            _operand = operand;
            _propertyName = name;
            _args = args?? EmptyOperands;
            _propertyInfo = operand.Type.GetProperty(
                name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, null, ToTypes(args)??Type.EmptyTypes, null);
            if (_propertyInfo == null)
            {
                if(args == null || args.Length == 0)
                {
                    throw new ArgumentException(string.Format("Not such property found: {0}.{1}", operand.Type, name));
                }
                StringBuilder sb = new StringBuilder("Not such indexer found: ");
                sb.Append(operand.Type).Append('[');
                foreach (var arg in args)
                {
                    sb.Append(arg.Type).Append(',');
                }
                if (args.Length > 0)
                {
                    sb.Length -= 1;
                }
                sb.Append(']');
                throw new ArgumentException(sb.ToString());
            }
        }

        public override Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            EmitArgs(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetGetMethod(true));
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            _operand.EmitGet(il);
            EmitArgs(il);
            value.EmitGet(il);
            il.Emit(OpCodes.Callvirt, _propertyInfo.GetSetMethod(true));
        }

        private void EmitArgs(ILGenerator il)
        {
            foreach (var operand in _args)
            {
                ((Operand)operand).EmitGet(il);
            }
        }
    }
}