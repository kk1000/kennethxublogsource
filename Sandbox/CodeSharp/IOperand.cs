using System;

namespace CodeSharp
{
    /// <summary>
    /// Represents an operand.
    /// </summary>
    public interface IOperand
    {
        /// <summary>
        /// Invoke a method on current operand.
        /// </summary>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="args">Parameters passed to the method.</param>
        /// <returns>
        /// The operand representing the result.
        /// </returns>
        IOperand Invoke(string methodName, params IOperand[] args);

        /// <summary>
        /// Access a property of current operand.
        /// </summary>
        /// <param name="name">
        /// The name of property.
        /// </param>
        /// <returns>
        /// The operent representing the property.
        /// </returns>
        IOperand Property(string name);

        /// <summary>
        /// The type of the operand.
        /// </summary>
        Type Type { get; }
    }
}