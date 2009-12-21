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
    /// Abstract list wrapper that implements most members of both 
    /// <see cref="IList{T}"/> and <see cref="IList"/>
    /// interfaces by delegating the calls to the wrapped list.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements in the list.
    /// </typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractListWrapper<T> : AbstractList<T>, IList<T>, IList
    {

        #region Protected Instance Properties

        /// <summary>
        /// Subclass must override and provide the wrapped list.
        /// </summary>
        protected abstract IList<T> WrappedList { get; }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        /// 
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.
        /// </param>
        public override int IndexOf(T item)
        {
            return WrappedList.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList{T}"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to insert into the <see cref="IList{T}"/>.</param>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IList{T}"/> is read-only.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.
        /// </exception>
        public override void Insert(int index, T item)
        {
            WrappedList.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="IList{T}"/> item at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the item to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IList{T}"/> is read-only.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.
        /// </exception>
        public override void RemoveAt(int index)
        {
            WrappedList.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
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
        public override T this[int index]
        {
            get { return WrappedList[index]; }
            set { WrappedList[index] = value; }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override void Add(T item)
        {
            WrappedList.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override void Clear()
        {
            WrappedList.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value.
        /// </summary>
        /// 
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(T item)
        {
            return WrappedList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index.
        /// </summary>
        /// 
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the 
        /// destination of the elements copied from <see cref="ICollection{T}"/>. 
        /// The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// array is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional.<br/>-or-<br/>
        /// arrayIndex is equal to or greater than the length of array. <br/>-or-<br/>
        /// The number of elements in the source <see cref="ICollection{T}"/> 
        /// is greater than the available space from arrayIndex to the end of 
        /// the destination array. <br/>-or-<br/>
        /// Type T cannot be cast automatically to the type of the destination array.
        /// </exception>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            WrappedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// This implementation always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if item was successfully removed from the <see cref="ICollection{T}"/>; 
        /// otherwise, false. This method also returns false if item is not found in the 
        /// original <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override bool Remove(T item)
        {
            return WrappedList.Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        public override int Count
        {
            get { return WrappedList.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="ICollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        /// 
        public override bool IsReadOnly
        {
            get { return WrappedList.IsReadOnly; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<T> GetEnumerator()
        {
            return WrappedList.GetEnumerator();
        }

        #endregion

        #region IList Members

        /// <summary>
        /// Called by implicit implementation of <see cref="IList.IsFixedSize"/>.
        /// </summary>
        protected override bool IsFixedSize
        {
            get {
                IList l = WrappedList as IList;
                return l != null && l.IsFixedSize;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index. Called by the implicit implementation of <see cref="ICollection.CopyTo"/>.
        /// </summary>
        ///
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination 
        /// of the elements copied from <see cref="ICollection"/>. The 
        /// <see cref="Array"/> must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        /// <exception cref="ArgumentNullException">array is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero. 
        /// </exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional.-or- index is equal to or greater than 
        /// the length of array.
        /// -or- 
        /// The number of elements in the source <see cref="ICollection"/> 
        /// is greater than the available space from index to the end of the 
        /// destination array. 
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ICollection"/> cannot be cast 
        /// automatically to the type of the destination array. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        protected override void CopyTo(Array array, int index)
        {
            IList c = WrappedList as IList;
            if (c != null)
            {
                c.CopyTo(array, index);
            }
            else
            {
                base.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> 
        /// is synchronized (thread safe).
        /// </summary>
        /// <remarks>
        /// This implementaiton always return <see langword="false"/>, unless the 
        /// wrapped list is also an instance of <see cref="IList"/>, in the case, 
        /// it returns the value from the wrapped list.
        /// </remarks>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"></see> 
        /// is synchronized (thread safe); otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        protected override bool IsSynchronized
        {
            get
            {
                IList c = WrappedList as IList;
                return c !=null && c.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the 
        /// <see cref="T:System.Collections.ICollection"></see>.
        /// </summary>
        /// <remarks>
        /// This implementation returns <see langword="null"/>, unless the 
        /// wrapped list is also an instance of <see cref="IList"/>, in this
        /// case, it returns the value from the wrapped list.
        /// </remarks>
        /// <returns>
        /// An object that can be used to synchronize access to the 
        /// <see cref="T:System.Collections.ICollection"></see>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        protected override object SyncRoot
        {
            get
            {
                IList c = WrappedList as IList;
                return c == null ? null : c.SyncRoot;
            }
        }

        #endregion

    }
}
