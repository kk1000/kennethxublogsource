namespace CodeSharp
{
    /// <summary>
    /// Represents a property.
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Mark the property public.
        /// </summary>
        IProperty Public { get; }

        /// <summary>
        /// Create the getter.
        /// </summary>
        /// <returns>
        /// The definition for the property getter.
        /// </returns>
        IInvokable Getter();

        /// <summary>
        /// Create the setter.
        /// </summary>
        /// <returns>
        /// The definition of the property getter.
        /// </returns>
        ISetter Setter();
    }
}