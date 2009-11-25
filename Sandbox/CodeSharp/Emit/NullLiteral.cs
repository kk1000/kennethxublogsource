using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class NullLiteral : Operand
    {
        private readonly Type _type;

        public NullLiteral(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
        }

        public override Type Type
        {
            get { return _type; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            il.Emit(OpCodes.Ldnull);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}