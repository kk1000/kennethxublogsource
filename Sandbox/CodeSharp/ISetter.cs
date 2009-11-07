namespace CodeSharp
{
    /// <summary>
    /// Represents a property setter.
    /// </summary>
    public interface ISetter : IInvokable
    {
        /// <summary>
        /// The value to set the property.
        /// </summary>
        IParameter Value { get; }
    }
}