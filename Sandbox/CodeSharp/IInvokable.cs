namespace CodeSharp
{
    /// <summary>
    /// Base type of <see cref="IMethod"/> and <see cref="IConstructor"/>
    /// </summary>
    public interface IInvokable
    {
        /// <summary>
        /// Get the <see cref="IMethodCode"/> associated with this method
        /// definition.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IMethodCode"/>.
        /// </returns>
        IMethodCode Code();

        /// <summary>
        /// Parameter list of the method.
        /// </summary>
        IParameterList Arg { get; }
    }
}