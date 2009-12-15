using System;

namespace CodeSharp
{
    /// <summary>
    /// Represents a parameter of method, delegate, indexer and etc.
    /// </summary>
    public interface IParameter : IOperand
    {
        /// <summary>
        /// The direction of parameter.
        /// </summary>
        ParameterDirection Direction { get; }

        /// <summary>
        /// Name of parameter.
        /// </summary>
        string Name { get; }
    }
}