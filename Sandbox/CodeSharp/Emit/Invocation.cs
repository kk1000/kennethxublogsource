using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Represents method invoation.
    /// </summary>
    class Invocation : Operand
    {
        private readonly Operand _operand;
        private readonly IEnumerable<IOperand> _args;
        private readonly MethodInfo _methodInfo;

        private Invocation(IOperand operand)
        {
            if (operand == null) throw new ArgumentNullException("operand");
            _operand = (Operand)operand;
        }

        public Invocation(IOperand operand, MethodInfo methodInfo, IEnumerable<IOperand> args)
            :this(operand)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            _methodInfo = methodInfo;
            _args = args;
        }

        /// <summary>
        /// Construct a new instance of <see cref="Invocation"/> that invokes
        /// method of given <paramref name="name"/> on given <paramref name="operand"/>
        /// with given <paramref name="args">parameters</paramref>.
        /// </summary>
        /// <param name="operand">
        /// The target to invoke the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The paremeter used to invoke the method.
        /// </param>
        public Invocation(IOperand operand, string name, IOperand[] args)
            :this(operand)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (args == null) args = EmptyOperands;
            _args = args;
            var paramTypes = new Type[args.Length];
            for (int i = args.Length - 1; i >= 0; i--)
            {
                paramTypes[i] = args[i].Type;
            }
            _methodInfo = operand.Type.GetMethod(
                name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, paramTypes, null);
            if (_methodInfo == null)
            {
                StringBuilder sb = new StringBuilder("Not such method found: ");
                sb.Append(operand.Type).Append('.').Append(name).Append('(');
                foreach (var type in paramTypes)
                {
                    sb.Append(type).Append(',');
                }
                if (paramTypes.Length > 0)
                {
                    sb.Length -= 1;
                }
                sb.Append(')');
                throw new ArgumentException(sb.ToString());
            }
        }

        /// <summary>
        /// The type of the operand.
        /// </summary>
        public override Type Type
        {
            get { return _methodInfo.ReturnType; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            Emit(il);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }

        internal void Emit(ILGenerator il)
        {
            if (!_methodInfo.IsStatic) _operand.EmitGet(il);
            foreach (var operand in _args)
            {
                ((Operand)operand).EmitGet(il);
            }
            il.Emit(OpCodes.Callvirt, _methodInfo);
        }
    }
}