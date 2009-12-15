using System.Reflection;

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
        /// Mark the property overrides given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">Property to be overriden.</param>
        IProperty Override(PropertyInfo property);

        /// <summary>
        /// Create the getter.
        /// </summary>
        /// <returns>
        /// The definition for the property getter.
        /// </returns>
        IMethod Getter();

        /// <summary>
        /// Create the setter.
        /// </summary>
        /// <returns>
        /// The definition of the property getter.
        /// </returns>
        ISetter Setter();
    }
}