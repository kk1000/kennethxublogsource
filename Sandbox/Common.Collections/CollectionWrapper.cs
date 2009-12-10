using System;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// <see cref="CollectionWrapper{T}"/> is a plain wrapper which does 
    /// nothing but delegate all memeber access to the wrapped collection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements in the collection.
    /// </typeparam>
    /// <author>Kenneth Xu</author>
    public class CollectionWrapper<T> : AbstractCollectionWrapper<T>
    {
        /// <summary>
        /// Construct a new instance of <see cref="CollectionWrapper{T}"/>
        /// that wraps the given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection to be wrapped.</param>
        /// <exception cref="ArgumentNullException">
        /// When paremeter <paramref name="collection"/> is <see langword="null"/>.
        /// </exception>
        public CollectionWrapper(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            WrappedInstance = collection;
        }

        /// <summary>
        /// Gets the wrapped collection, which is <see cref="WrappedInstance"/>.
        /// </summary>
        protected override ICollection<T> WrappedCollection
        {
            get { return WrappedInstance; }
        }

        /// <summary>
        /// The wrapped collection.
        /// </summary>
        protected ICollection<T> WrappedInstance;

        #region Object Methods

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// <see cref="object.GetHashCode"/> is suitable for use in 
        /// hashing algorithms and data structures like a hash table.
        /// </summary>
        ///
        /// <returns>
        /// The hash code of the wrapped collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return WrappedCollection.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> 
        /// is equal to the current <see cref="CollectionWrapper{T}"/>.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is an instance 
        /// of <see cref="CollectionWrapper{T}"/> and its wrapped collection 
        /// is equal to the wrapped collection of current 
        /// <see cref="CollectionWrapper{T}"/>; otherwise, <c>false</c>.
        /// </returns>
        ///
        /// <param name="obj">
        /// The <see cref="object"/> to compare with the current 
        /// <see cref="CollectionWrapper{T}"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            CollectionWrapper<T> c = obj as CollectionWrapper<T>;
            return c!=null && WrappedCollection.Equals(c.WrappedCollection);
        }

        #endregion

    }
}
