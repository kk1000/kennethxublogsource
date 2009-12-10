using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Collection
{
    /// <summary>
    /// <see cref="ListWrapper{T}"/> is a plain wrapper which does 
    /// nothing but delegate all memeber access to the wrapped list.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements in the collection.
    /// </typeparam>
    /// <author>Kenneth Xu</author>
    public class ListWrapper<T> : AbstractListWrapper<T>
    {
        /// <summary>
        /// The wrapped list.
        /// </summary>
        protected IList<T> WrappedInstance;

        /// <summary>
        /// Construct a new instance of <see cref="ListWrapper{T}"/>
        /// that wraps the given <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to be wrapped.</param>
        /// <exception cref="ArgumentNullException">
        /// When paremeter <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public ListWrapper(IList<T> list)
        {
            if (list==null) throw new ArgumentNullException("list");
            WrappedInstance = list;
        }

        /// <summary>
        /// Gets the wrapped list, which is <see cref="WrappedInstance"/>.
        /// </summary>
        protected override IList<T> WrappedList
        {
            get { return WrappedInstance; }
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// <see cref="object.GetHashCode"/> is suitable for use in 
        /// hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// The hash code of the wrapped list.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return WrappedList.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal 
        /// to the current <see cref="ListWrapper{T}"/>.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is an instance 
        /// of <see cref="ListWrapper{T}"/> and its wrapped list is equal to 
        /// the wrapped collection of current <see cref="ListWrapper{T}"/>; 
        /// otherwise, <c>false</c>.
        /// </returns>
        ///
        /// <param name="obj">
        /// The <see cref="object"/> to compare with the current 
        /// <see cref="ListWrapper{T}"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            ListWrapper<T> c = obj as ListWrapper<T>;
            return c != null && WrappedList.Equals(c.WrappedList);
        }

    }
}
