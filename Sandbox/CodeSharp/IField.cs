namespace CodeSharp
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    public interface IField : IOperand
    {
        /// <summary>
        /// Mark field readonly.
        /// </summary>
        IField ReadOnly { get; }

        /// <summary>
        /// Mark field internal.
        /// </summary>
        IField Internal { get; }
    }
}