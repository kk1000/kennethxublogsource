#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// Abstract collection wrapper that implements most of both 
    /// <see cref="ICollection{T}"/> and <see cref="ICollection"/>
    /// interfaces by delegating the calls to the wrapped collection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements in the collection.
    /// </typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractCollectionWrapper<T> : AbstractCollection<T>, ICollection<T>, ICollection
    {
        /// <summary>
        /// Subclass must override and provide the wrapped collection.
        /// </summary>
        protected abstract ICollection<T> WrappedCollection
        {
            get;
        }

        #region ICollection<T> Members

        ///<summary>
        ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation adds the item to the wrapped collection.</remarks>
        ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        ///<exception cref="T:System.NotSupportedException">The wrapped <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public override void Add(T item)
        {
            WrappedCollection.Add(item);
        }

        ///<summary>
        ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation clears the wrapped collection.</remarks>
        ///<exception cref="T:System.NotSupportedException">The wrapped <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public override void Clear()
        {
            WrappedCollection.Clear();
        }

        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        ///</summary>
        ///<remarks>This implementation delegates the call to the wrapped collection.</remarks>
        ///<returns>
        ///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public override bool Contains(T item)
        {
            return WrappedCollection.Contains(item);
        }

        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        ///</summary>
        ///<remarks>This implementation delegates the call to the wrapped object.</remarks>
        ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        ///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        ///<exception cref="T:System.ArgumentNullException">array is null.</exception>
        ///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            WrappedCollection.CopyTo(array, arrayIndex);
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation returns the count of the wrapped collection.</remarks>
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</returns>
        ///
        public override int Count
        {
            get { return WrappedCollection.Count; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        ///</summary>
        ///<remarks>return <see langword="true"/> when the wrapped collection returns <c>true</c></remarks>
        ///<returns>
        ///true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
        ///</returns>
        ///
        public override bool IsReadOnly
        {
            get { return WrappedCollection.IsReadOnly; }
        }

        ///<summary>
        ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation removes the item from the wrapped collection</remarks>
        ///<returns>
        ///true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</returns>
        ///
        ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        ///<exception cref="T:System.NotSupportedException">The wrapped <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public override bool Remove(T item)
        {
            return WrappedCollection.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///<remarks>returns the enumerator from the wrapped collection.</remarks>
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public override IEnumerator<T> GetEnumerator()
        {
            return WrappedCollection.GetEnumerator();
        }

        #endregion

        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
        ///<param name="index">The zero-based index in array at which copying begins. </param>
        ///<exception cref="T:System.ArgumentNullException">array is null. </exception>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        ///<exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        ///<exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception><filterpriority>2</filterpriority>
        protected override void CopyTo(Array array, int index)
        {
            ICollection<T> c = WrappedCollection;
            if (c is ICollection)
            {
                ((ICollection)c).CopyTo(array, index);
            }
            else
            {
                base.CopyTo(array, index);
            }
        }

        ///<summary>
        ///Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
        ///</summary>
        ///<remarks>
        /// Returns <c>true</c> only when the wrapped collection implements 
        /// <see cref="ICollection"/> interface and its <see cref="ICollection.IsSynchronized"/>
        /// property is <c>true</c>
        /// </remarks>
        ///<returns>
        ///true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override bool IsSynchronized
        {
            get
            {
                ICollection<T> c = WrappedCollection;
                return c is ICollection && ((ICollection)c).IsSynchronized;
            }
        }

        ///<summary>
        ///Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</summary>
        ///<remarks>Returns the <c>SyncRoot</c> of the wrapped collection only 
        /// it implements the <see cref="ICollection"/> interface. Otherwise <c>null</c>.
        /// </remarks>
        ///<returns>
        ///An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override object SyncRoot
        {
            get
            {
                ICollection<T> c = WrappedCollection;
                return c is ICollection ? ((ICollection)c).SyncRoot : null;
            }
        }

    }
}
