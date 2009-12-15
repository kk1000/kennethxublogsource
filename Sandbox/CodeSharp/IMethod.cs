using System.Reflection;

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

        /// <summary>
        /// Define current method as static.
        /// </summary>
        IMethod Static { get; }

        /// <summary>
        /// Current method overrides <paramref name="method"/>
        /// </summary>
        /// <param name="method">
        /// The method to be overidden.
        /// </param>
        /// <returns>
        /// Current Method.
        /// </returns>
        IMethod Override(MethodInfo method);
    }
}