using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal class Variable : Operand
    {
        private readonly Type _type;
        private readonly string _name;
        private LocalBuilder _variable;

        public Variable(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        public override Type Type
        {
            get { return _type; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            switch (_variable.LocalIndex)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            value.EmitGet(il);
            switch (_variable.LocalIndex)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void EmitDefinition(ILGenerator il)
        {
            _variable = il.DeclareLocal(_type);
        }
    }
}