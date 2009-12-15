using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Operand definition
    /// </summary>
    internal abstract class Operand : IOperand
    {
        public IOperand Field(string name)
        {
            return new FieldAccess(this, name);
        }

        public IOperand Field(IField field)
        {
            return new FieldAccess(this, field);
        }

        public readonly static Operand[] EmptyOperands = new Operand[0];

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

        public IOperand Invoke(MethodInfo methodInfo, IEnumerable<IOperand> args)
        {
            return new Invocation(this, methodInfo, args);
        }

        public IOperand Invoke(MethodInfo methodInfo, params IOperand[] args)
        {
            return Invoke(methodInfo, (IEnumerable<IOperand>) args);
        }

        public IOperand Property(string name)
        {
            return new PropertyAccess(this, name);
        }

        public IOperand Indexer(params IOperand[] args)
        {
            return new PropertyAccess(this, args);
        }

        public IOperand Property(PropertyInfo propertyInfo, IEnumerable<IOperand> indexes)
        {
            return new PropertyAccess(this, propertyInfo, indexes);
        }

        public IOperand Property(PropertyInfo propertyInfo, params IOperand[] indexes)
        {
            return new PropertyAccess(this, propertyInfo, indexes);
        }

        public IOperand As(Type type)
        {
            return new SafeCast(this, type);
        }

        public IOperand As(IClass @class)
        {
            return new SafeCast(this, @class);
        }

        /// <summary>
        /// The type of the operand.
        /// </summary>
        public abstract Type Type { get; }

        internal abstract void EmitGet(ILGenerator il);
        internal abstract void EmitSet(ILGenerator il, Operand value);

        internal virtual void EmitByRef(ILGenerator il)
        {
            throw new InvalidOperationException();
        }

        internal static Type[] ToTypes(IList<IOperand> operands)
        {
            if (operands == null) return null;
            var types = new Type[operands.Count];
            int i = 0;
            foreach (var operand in operands)
            {
                types[i++] = operand.Type;
            }
            return types;
        }
    }

    internal class SafeCast : Operand
    {
        private readonly Operand _operand;
        private readonly Type _type;
        private readonly Class _class;

        public SafeCast(Operand operand, Type type)
        {
            _operand = operand;
            _type = type;
        }

        public SafeCast(Operand operand, IClass @class)
        {
            _operand = operand;
            _class = (Class)@class;
        }

        public override Type Type
        {
            get { return _type??_class.TypeBuilder; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            _operand.EmitGet(il);
            il.Emit(OpCodes.Isinst, Type);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}