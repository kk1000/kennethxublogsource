using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CodeSharp.Emit.Conditions;

namespace CodeSharp.Emit
{
    /// <summary>
    /// To define the code in a class memebers.
    /// </summary>
    class Code : CodeBlock, ICode
    {
        readonly IList<Variable> _variables = new List<Variable>();
        /// <summary>
        /// Complete the code of the method and prevent further modification.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        public IOperand Variable(Type type, string name)
        {
            Variable variable = new Variable(type, name);
            _variables.Add(variable);
            return variable;
        }

        /// <summary>
        /// Return the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value to return.
        /// </param>
        public void Return(IOperand value)
        {
            AddCodeSnip(new ReturnCode((Operand)value));
        }

        /// <summary>
        /// Return.
        /// </summary>
        public void Return()
        {
            AddCodeSnip(new ReturnCode());
        }

        /// <summary>
        /// Assign the <paramref name="value"/> to <paramref name="target"/>
        /// </summary>
        /// <param name="target">
        /// Target to assign the value.
        /// </param>
        /// <param name="value">
        /// Value to be assigned.
        /// </param>
        public void Assign(IOperand target, IOperand value)
        {
            AddCodeSnip(new AssignmentCode((Operand)target, (Operand)value));
        }

        public void Call(IOperand statement)
        {
            AddCodeSnip(new CallCode((Operand)statement));
        }

        public IOperand New(Type type, params IOperand[] args)
        {
            return new Construction(type, args);
        }

        public ICondition AreNotEqual(IOperand left, IOperand right)
        {
            return new ConditionNot(new Equals((Operand)left, (Operand)right));
        }

        public ICondition IsNull(IOperand operand)
        {
            return new IsNull((Operand) operand);
        }

        public ICondition NotReferenceEquals(IOperand operand1, IOperand operand2)
        {
            return new ConditionNot(new ReferenceEquals((Operand) operand1, (Operand) operand2));
        }

        public ICondition Or(ICondition condition1, ICondition condition2)
        {
            return new ConditionOr((Condition) condition1, (Condition) condition2);
        }

        public void If(ICondition condition)
        {
            StartBlock(new IfBlock((Condition)condition));
        }

        public void End()
        {
            EndBlock();
        }

        /// <summary>
        /// Emit IL code for method.
        /// </summary>
        /// <param name="il">
        /// The method to emit the code for.
        /// </param>
        public override void Emit(ILGenerator il)
        {
            foreach (var variable in _variables)
            {
                variable.EmitDefinition(il);
            }
            foreach (var code in _codes)
            {
                code.Emit(il);
            }
        }
    }
}