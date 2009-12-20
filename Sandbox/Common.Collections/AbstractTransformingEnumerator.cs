using System;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// Provide basic functions to construct a new strong typed enumerator
    /// of type <typeparamref name="TTo"/> from anothe strong typed enumerator
    /// of type <typeparamref name="TFrom"/> without copying the elements.
    /// </summary>
    /// <typeparam name="TTo">the element type of the constructed enumerator.</typeparam>
    /// <typeparam name="TFrom">the element type of the source enumerator.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractTransformingEnumerator<TFrom, TTo> : AbstractEnumerator<TTo>
    {
        /// <summary>
        /// Base enumerator to be transformed.
        /// </summary>
        protected readonly IEnumerator<TFrom> _source;

        /// <summary>
        /// Constructor a new enumerator transformed from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The enumerator to be transformed.</param>
        protected AbstractTransformingEnumerator(IEnumerator<TFrom> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            _source = source;
        }

        ///<summary>
        ///Disposes the enumerator causes the source enumerator to be disposed 
        /// as well.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public override void Dispose()
        {
            _source.Dispose();
        }

        ///<summary>
        ///Sets the enumerator to its initial position, which is before the 
        /// first element in the collection.
        ///</summary>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override void Reset()
        {
            _source.Reset();
        }

        ///<summary>
        ///Advances the enumerator to the next element of the collection.
        ///</summary>
        ///
        ///<returns>
        ///true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the 
        /// collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        protected override bool GoNext()
        {
            return _source.MoveNext();
        }

        ///<summary>
        ///Gets the element in the collection at the current position of the 
        /// enumerator.
        ///</summary>
        ///
        ///<returns>
        ///The element in the collection at the current position of the 
        /// enumerator.
        ///</returns>
        ///
        protected override TTo FetchCurrent()
        {
            return Transform(_source.Current);
        }

        /// <summary>
        /// Converts object of type <typeparamref name="TFrom"/> to
        /// <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="source">
        /// Instace of <typeparamref name="TFrom"/> to be converted.
        /// </param>
        /// <returns>
        /// Converted object of type <typeparamref name="TTo"/>.
        /// </returns>
        protected abstract TTo Transform(TFrom source);
    }
}