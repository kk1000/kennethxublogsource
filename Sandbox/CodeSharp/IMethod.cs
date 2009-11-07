namespace CodeSharp
{
    /// <summary>
    /// Represents a method.
    /// </summary>
    public interface IMethod : IInvokable
    {
        /// <summary>
        /// Allow public access to current method.
        /// </summary>
        IMethod Public { get; }
    }
}