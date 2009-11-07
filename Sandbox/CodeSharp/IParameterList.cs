namespace CodeSharp
{
    /// <summary>
    /// Represents parameter list of method, indexer, delegate and etc.
    /// </summary>
    public interface IParameterList
    {
        /// <summary>
        /// Gets the <see cref="IParameter">parameter</see> at given position.
        /// </summary>
        /// <param name="position">
        /// Position of the parameter in the parameter list.
        /// </param>
        /// <returns>
        /// The parameter at given position.
        /// </returns>
        IParameter this[int position] { get; }
    }
}