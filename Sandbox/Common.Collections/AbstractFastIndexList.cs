using System;
using System.Collections.Generic;
using System.Collections;

namespace Common.Collection
{
    /// <summary>
    /// Serve as based class to be inherited by the classes that needs to
    /// implement both the <see cref="IList{T}"/> and the <see cref="IList"/>
    /// interfaces.
    /// </summary>
    /// <typeparam name="T">Element type of the collection</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractFastIndexList<T> : AbstractList<T>
    {
        /// <summary>
        /// Gets the number of elements contained in this list.
        /// Subclass must implement this property.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements contained in this list.
        /// </returns>
        /// 
        public abstract override int Count
        {
            get;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the list.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<T> GetEnumerator()
        {
            return new FastIndexListEnumerator<T>(this);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>
        /// Concrete subclass must implement this indexer.
        /// </remarks>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// 
        /// <param name="index">
        /// The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The property is set and the 
        /// <see cref="IList{T}"/> is read-only.</exception>
        public abstract override T this[int index]
        {
            get;
            set;
        }
    }

        /// <summary>
        /// Enumerator for implementation of the <see cref="IList{T}"/> 
        /// that support fast indexing.
        /// </summary>
        /// <typeparam name="T">Type of the enumerator type.</typeparam>
    internal sealed class FastIndexListEnumerator<T> : AbstractEnumerator<T>
    {
        int _index = -1;
        readonly IList<T> _list;

        /// <summary>
        /// Create a new instance of enumerator based on <paramref name="list"/>.
        /// </summary>
        /// <param name="list">
        /// The list that the enumerator that is based on.
        /// </param>
        public FastIndexListEnumerator(IList<T> list)
        {
            this._list = list;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before 
        /// the first element in the collection. This implementation
        /// always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Always thown.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override void Reset()
        {
            _index = -1;
        }

        #endregion

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        ///
        /// <returns>
        /// true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the collection.
        /// </returns>
        ///
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        protected override bool GoNext()
        {
            return ++_index < _list.Count;
        }

        /// <summary>
        /// Fetch the current element of the enumerator.
        /// </summary>
        /// <returns>The current element</returns>
        protected override T FetchCurrent()
        {
            return _list[_index];
        }
    }
    
}
