namespace Common.Collection
{
    /// <summary>
    /// Enumerator for a single element.
    /// </summary>
    /// <typeparam name="T">Type of the elements to be iterated.</typeparam>
    /// <authro>Kenneth Xu</authro>
    public class SingletonEnumerator<T> : AbstractEnumerator<T>
    {
        private readonly T _theSingleton;

        /// <summary>
        /// Construct an emuerator for the single item <paramref name="o"/>.
        /// </summary>
        /// <param name="o">The only elemnt of the enumerator.</param>
        public SingletonEnumerator(T o)
        {
            _theSingleton = o;
        }

        /// <summary>
        /// Fetch the current element of the enumerator.
        /// </summary>
        /// <returns>The current element</returns>
        protected override T FetchCurrent()
        {
            return _theSingleton;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        ///
        /// <returns>
        /// true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the collection.
        /// </returns>
        protected override bool GoNext()
        {
            return (State == EnumeratorState.BeforeStart);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before 
        /// the first element in the collection.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Reset()
        {
            State = EnumeratorState.BeforeStart;
        }

    }
}