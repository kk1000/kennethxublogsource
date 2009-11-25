using System;
using System.Reflection;

namespace CodeSharp
{
    /// <summary>
    /// Represents the code of a method, constructor or property.
    /// </summary>
    public interface ICode : IDisposable
    {
        /// <summary>
        /// Complete the code of the method and prevent further modification.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        new void Dispose();

        /// <summary>
        /// Return the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value to return.
        /// </param>
        void Return(IOperand value);

        /// <summary>
        /// Return.
        /// </summary>
        void Return();

        /// <summary>
        /// Assign the <paramref name="value"/> to <paramref name="target"/>
        /// </summary>
        /// <param name="target">
        /// Target to assign the value.
        /// </param>
        /// <param name="value">
        /// Value to be assigned.
        /// </param>
        void Assign(IOperand target, IOperand value);

        /// <summary>
        /// Execute expression.
        /// </summary>
        /// <param name="operand"></param>
        void Call(IOperand operand);

        /// <summary>
        /// Create a new instance of object.
        /// </summary>
        /// <param name="type">
        /// Type of the object to create.
        /// </param>
        /// <param name="args">
        /// Argument passed to the constructor.
        /// </param>
        /// <returns></returns>
        IOperand New(Type type, params IOperand[] args);

        /// <summary>
        /// left != right
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>
        /// Condition of: left != right
        /// </returns>
        ICondition AreNotEqual(IOperand left, IOperand right);

        /// <summary>
        /// !ReferenceEquals(operand1, operand2)
        /// </summary>
        /// <param name="operand1">First operand to compare.</param>
        /// <param name="operand2">Second operand to compare.</param>
        /// <returns>
        /// Condition of: !ReferenceEquals(operand1, operand2)
        /// </returns>
        ICondition NotReferenceEquals(IOperand operand1, IOperand operand2);

        /// <summary>
        /// condition1 || condition2
        /// </summary>
        /// <param name="condition1">First condition.</param>
        /// <param name="condition2">Second condition.</param>
        /// <returns>
        /// Condition of: condition1 || condition2
        /// </returns>
        ICondition Or(ICondition condition1, ICondition condition2);

        /// <summary>
        /// operand == null
        /// </summary>
        /// <param name="operand">
        /// The operand to check for null.
        /// </param>
        /// <returns>
        /// Condition of: operand == null
        /// </returns>
        ICondition IsNull(IOperand operand);

        /// <summary>
        /// start of if statement block
        /// </summary>
        /// <param name="condition">
        /// condition of if statement.
        /// </param>
        void If(ICondition condition);

        /// <summary>
        /// End of code block
        /// </summary>
        void End();

        /// <summary>
        /// Define a local variable.
        /// </summary>
        /// <param name="type">Type of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <returns>
        /// An <see cref="IOperand"/> representing the variable.
        /// </returns>
        IOperand Variable(Type type, string name);
    }
}