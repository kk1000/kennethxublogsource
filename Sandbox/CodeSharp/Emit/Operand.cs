using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Operand definition
    /// </summary>
    abstract class Operand : IOperand
    {
        public readonly static IOperand[] EmptyOperands = new IOperand[0];

        /// <summary>
        /// Invoke a method on current operand.
        /// </summary>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="args">Parameters passed to the method.</param>
        /// <returns>
        /// The operand representing the result.
        /// </returns>
        public IOperand Invoke(string methodName, params IOperand[] args)
        {
            return new Invocation(this, methodName, args);
        }

        public IOperand Property(string name)
        {
            return new PropertyAccess(this, name);
        }

        public IOperand Indexer(params IOperand[] args)
        {
            return new PropertyAccess(this, args);
        }

        /// <summary>
        /// The type of the operand.
        /// </summary>
        public abstract Type Type { get; }

        internal abstract void EmitGet(ILGenerator il);
        internal abstract void EmitSet(ILGenerator il, Operand value);

        internal static Type[] ToTypes(IOperand[] operands)
        {
            if (operands == null) return null;
            var types = new Type[operands.Length];
            for (int i = operands.Length - 1; i >= 0; i--)
            {
                types[i] = operands[i].Type;
            }
            return types;
        }
    }
}