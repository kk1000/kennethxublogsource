using System;

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
    }
}