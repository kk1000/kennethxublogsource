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

using System.Collections.Generic;
using System.Extension;
using Common;

namespace Common.Collection
{
    /// <summary>
    /// A collection class that wraps another collection and can be sealed.
    /// </summary>
    /// <typeparam name="T">Type of the collection element.</typeparam>
    /// <author>Kenneth Xu</author>
    public class SealableCollection<T> : CollectionWrapper<T>, ISealableCollection<T>, ISealableCollection
    {
        /// <summary>
        /// A volatile flag to indicate that the instance is sealed.
        /// </summary>
        protected volatile bool m_Sealed = false;

        /// <summary>
        /// Construct a new instance wraps the given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">
        /// The original collection to be wrapped as sealable.
        /// </param>
        public SealableCollection(ICollection<T> collection) : base(collection) {}

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
        /// <exception cref="InstanceSealedException">when instance is sealed</exception>
        protected void FailIfSealed()
        {
            if (m_Sealed) throw new InstanceSealedException();
        }


        #region ICollection<T> Members

        ///<summary>
        ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation adds the item to the wrapped collection.</remarks>
        ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        ///<exception cref="T:System.NotSupportedException">The wrapped <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        ///<exception cref="InstanceSealedException">When <see cref="IsSealed"/> is <see langword="true"/>.</exception>
        public override void Add(T item)
        {
            FailIfSealed();
            base.Add(item);
        }

        ///<summary>
        ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        ///</summary>
        ///<remarks>This implementation clears the wrapped collection.</remarks>
        ///<exception cref="T:System.NotSupportedException">The wrapped <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        ///<exception cref="InstanceSealedException">When <see cref="IsSealed"/> is <see langword="true"/>.</exception>
        public override void Clear()
        {
            FailIfSealed();
            base.Clear();
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        ///</summary>
        ///<remarks>return <see langword="true"/> when the wrapped collection returns <c>true</c> or 
        /// <see cref="IsSealed"/> is <see langword="true"/>.</remarks>
        ///<returns>
        ///true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
        ///</returns>
        ///
        public override bool IsReadOnly
        {
            get { return m_Sealed || WrappedInstance.IsReadOnly; }
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
        ///<exception cref="InstanceSealedException">When <see cref="IsSealed"/> is <see langword="true"/>.</exception>
        public override bool Remove(T item)
        {
            FailIfSealed();
            return base.Remove(item);
        }

        #endregion

    }
}
