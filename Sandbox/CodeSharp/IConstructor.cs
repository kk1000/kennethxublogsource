namespace CodeSharp
{
    /// <summary>
    /// Represents an constructor.
    /// </summary>
    public interface IConstructor : IInvokable
    {
        /// <summary>
        /// Allow public access to current constructor.
        /// </summary>
        IConstructor Public { get; }
    }
}