namespace System.Extension
{
    /// <summary>
    /// Interface for objects that support indexing 
    /// </summary>
    public interface IIndexable
    {
        /// <summary>
        /// Gets or sets the data with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the data to get or set</param>
        /// <returns>The data with the specified <paramref name="index"/></returns>
        object this[object index] { get;set;}
    }

    /// <summary>
    /// Generic version of <see cref="IIndexable"/>.
    /// </summary>
    /// <typeparam name="TIndex">Type of the index</typeparam>
    /// <typeparam name="TValue">Type of the data value</typeparam>
    public interface IIndexable<TIndex, TValue>
    {
        /// <summary>
        /// Gets or sets the data with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the data to get or set</param>
        /// <returns>The data with the specified <paramref name="index"/></returns>
        TValue this[TIndex index] { get; set;}
    }
}
