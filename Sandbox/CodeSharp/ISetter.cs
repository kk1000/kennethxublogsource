using System.Reflection;

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

        /// <summary>
        /// Current method overrides the method
        /// </summary>
        /// <param name="method">
        /// The method to be overridden.
        /// </param>
        ISetter Override(MethodInfo method);
    }
}