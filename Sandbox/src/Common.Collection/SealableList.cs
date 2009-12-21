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
using System.Collections.Generic;
using System.Extension;

namespace Common.Collection
{
    /// <summary>
    /// A list class that wraps another list and can be sealed.
    /// </summary>
    /// <typeparam name="T">Type of the list element.</typeparam>
    /// <author>Kenneth Xu</author>
    public class SealableList<T> : ListWrapper<T>, ISealableList<T>, ISealableList
    {
        /// <summary>
        /// A volatile flag to indicate that the instance is sealed.
        /// </summary>
        protected volatile bool m_Sealed = false;

        /// <summary>
        /// Construct a new instance wraps the given <paramref name="list"/>.
        /// </summary>
        /// <param name="list">
        /// The original collection to be wrapped as sealable.
        /// </param>
        public SealableList(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Construct a new instance wraps a new instance of
        /// <see cref="List{T}"/>.
        /// </summary>
        public SealableList() : this(new List<T>()) 
        {
        }

        #region ISealable Members

        /// <summary>
        /// implements <see cref="ISealable.Seal"/>
        /// </summary>
        public virtual void Seal()
        {
            m_Sealed = true;
        }

        /// <summary>
        /// implements <see cref="ISealable.IsSealed"/>
        /// </summary>
        public bool IsSealed
        {
            get { return m_Sealed; }
        }

        #endregion

        /// <summary>
        /// A convenient method that throws <c>InstanceSealedException</c> when
        /// the instance is sealed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See documentation of the class <see cref="Sealable"/> for an example.
        /// </para>
        /// </remarks>
        /// <exception cref="InstanceSealedException">
        /// when instance is sealed
        /// </exception>
        protected void FailIfSealed()
        {
            if (m_Sealed) throw new InstanceSealedException();
        }

        #region IList<T> Members

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override void Add(T item)
        {
            FailIfSealed();
            WrappedInstance.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override void Clear()
        {
            FailIfSealed();
            WrappedInstance.Clear();
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
        /// The <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.
        /// </exception>
        public override void Insert(int index, T item)
        {
            FailIfSealed();
            WrappedInstance.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="IList{T}"/> item at the specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The zero-based index of the item to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.
        /// </exception>
        public override void RemoveAt(int index)
        {
            FailIfSealed();
            WrappedInstance.RemoveAt(index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the 
        /// <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if item was successfully removed from the <see cref="IList{T}"/>; 
        /// otherwise, false. This method also returns false if item is not found 
        /// in the original <see cref="IList{T}"/>.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to remove from the <see cref="IList{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override bool Remove(T item)
        {
            FailIfSealed();
            return WrappedInstance.Remove(item);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IList{T}"/> is read-only.
        /// </summary>
        /// 
        /// <returns>
        /// true if <see cref="IsSealed"/> is <see langword="true"/> or the
        /// <see cref="ListWrapper{T}.WrappedInstance"/> is read-only; 
        /// otherwise, false.
        /// </returns>
        /// 
        public override bool IsReadOnly
        {
            get { return m_Sealed || WrappedInstance.IsReadOnly; }
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
        /// <exception cref="NotSupportedException">
        /// The property is set and the 
        /// <see cref="ListWrapper{T}.WrappedInstance"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// The property is set and the 
        /// <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override T this[int index]
        {
            get
            {
                return WrappedInstance[index];
            }
            set
            {
                FailIfSealed();
                WrappedInstance[index] = value;
            }
        }

        #endregion

    }
}
