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

namespace Common.Collection
{
    /// <summary>
    /// A list that is consisted of a number of other <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">type of the items that the list contains</typeparam>
    /// <author>Kenneth Xu</author>
    public class CompositeList<T> : AbstractList<T>
    {
        private readonly List<IList<T>> _lists;

        #region Operator Overloading


        /// <summary>
        /// Add a list <paramref name="c"/> to the <see cref="CompositeList{T}"/>
        /// <paramref name="cc"/>.
        /// </summary>
        /// <param name="cc">The <see cref="CompositeList{T}"/>.</param>
        /// <param name="c">The list to be added.</param>
        /// <returns>
        /// The same <see cref="CompositeList{T}"/> <paramref name="cc"/>
        /// </returns>
        public static CompositeList<T> operator 
            +(CompositeList<T> cc, IList<T> c)
        {
            cc.AddList(c);
            return cc;
        }

        /// <summary>
        /// Insert a list <paramref name="c"/> to the beginning of the 
        /// <see cref="CompositeList{T}"/> <paramref name="cc"/>.
        /// </summary>
        /// <param name="cc">The <see cref="CompositeList{T}"/>.</param>
        /// <param name="c">The list to be inserted.</param>
        /// <returns>
        /// The same <see cref="CompositeList{T}"/> <paramref name="cc"/>
        /// </returns>
        public static CompositeList<T> operator 
            +(IList<T> c, CompositeList<T> cc)
        {
            cc.InsertList(0, c);
            return cc;
        }

        /// <summary>
        /// Add a range of lists to a <see cref="CompositeList{T}"/>.
        /// </summary>
        /// <param name="cc">A <see cref="CompositeList{T}"/>.</param>
        /// <param name="ec">A range of enumerators to be added.</param>
        /// <returns>
        /// The same <see cref="CompositeList{T}"/> <paramref name="cc"/>
        /// </returns>
        public static CompositeList<T> operator 
            +(CompositeList<T> cc, IEnumerable<IList<T>> ec)
        {
            cc.AddLists(ec);
            return cc;
        }

        /// <summary>
        /// Insert a range of lists to a <see cref="CompositeList{T}"/>.
        /// </summary>
        /// <param name="cc">A <see cref="CompositeList{T}"/>.</param>
        /// <param name="ec">A range of enumerators to be inserted.</param>
        /// <returns>
        /// The same <see cref="CompositeList{T}"/> <paramref name="cc"/>
        /// </returns>
        public static CompositeList<T> operator 
            +(IEnumerable<IList<T>> ec, CompositeList<T> cc)
        {
            cc.InsertLists(0, ec);
            return cc;
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Construct an empty list that other lists can
        /// be added to it later.
        /// </summary>
        public CompositeList()
            : this(null, 0)
        {
        }

        /// <summary>
        /// Construct a list with the given initial <paramref name="capacity"/>
        /// that other lists can be added to it later.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeList(int capacity)
            : this(null, capacity)
        {
        }


        /// <summary>
        /// Construct a list that is composite of multiple 
        /// <paramref name="lists"/>. More lists can be added later.
        /// </summary>
        /// <param name="lists">
        /// Lists that this <see cref="CompositeList{T}"/> initially contains.
        /// </param>
        public CompositeList(IEnumerable<IList<T>> lists)
            : this(lists, 0)
        {
        }

        /// <summary>
        /// Construct a list with the given initial <paramref name="capacity"/> 
        /// that is composite of multiple <paramref name="lists"/>. More 
        /// lists can be added to it later.
        /// </summary>
        /// <param name="lists">
        /// Lists that this <see cref="CompositeList{T}"/> initially contains.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeList(IEnumerable<IList<T>> lists, int capacity)
        {
            _lists = capacity > 0 ? 
                new List<IList<T>>(capacity) : 
                new List<IList<T>>();

            if (lists != null) AddLists(lists);
        }

        #endregion

        /// <summary>
        /// Add the <paramref name="list"/> to the end of composition.
        /// </summary>
        /// <param name="list">
        /// The list to be added to the end of the composition.
        /// </param>
        public void AddList(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");
            _lists.Add(list);
        }

        /// <summary>
        /// Insert the <paramref name="list"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="list">The list to be inserted.</param>
        public void InsertList(int position, IList<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");
            _lists.Insert(position, list);
        }

        /// <summary>
        /// Add a range of <paramref name="lists"/> to the end of the composition.
        /// </summary>
        /// <param name="lists">
        /// The lists to be added to the end of the composition.
        /// </param>
        public void AddLists(IEnumerable<IList<T>> lists)
        {
            if (lists == null) throw new ArgumentNullException("lists");
            EnsureNoNull(lists);
            _lists.AddRange(lists);
        }

        /// <summary>
        /// Insert a range of <paramref name="lists"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="lists">The lists to be inserted.</param>
        public void InsertLists(int position, IEnumerable<IList<T>> lists)
        {
            if (lists == null) throw new ArgumentNullException("lists");
            EnsureNoNull(lists);
            _lists.InsertRange(position, lists);
        }



        #region IList<T> Members

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The getter exams the <see cref="ICollection{T}.Count"/> of each 
        /// composited list and retrieves the item from the appropriate
        /// list.
        /// </para>
        /// <para>
        /// The setter throws the <see cref="NotSupportedException"/>.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the <see cref="IList{T}"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="IList{T}"/> is read-only.
        /// </exception>
        public override T this[int index]
        {
            get
            {
                if (index < 0) throw new ArgumentOutOfRangeException(
                      "index", index, "cannot be less then zero.");
                int i = index;
                foreach (IList<T> ts in _lists)
                {
                    int count = ts.Count;
                    if (i < count) return ts[i];
                    i -= count;
                }
                throw new ArgumentOutOfRangeException(
                        "index", index, "list has only " + (index-i) + " elements");
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// This implementation search item in each composited list in this 
        /// composition.
        /// </summary>
        /// 
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="IList{T}"/>.
        /// </param>
        public override int IndexOf(T item)
        {
            int count = 0;

            foreach (IList<T> c in _lists)
            {
                int index = c.IndexOf(item);
                if (index >= 0) return count + index;
                count += c.Count;
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the <see cref="IList{T}"/> contains a specific 
        /// value.
        /// </summary>
        /// <remarks>
        /// This implementation check all the lists that consists this composite
        /// list to see if any of them contains the given <paramref name="item"/>.
        /// </remarks>
        /// <returns>
        /// true if item is found in the <see cref="IList{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="IList{T}"/>.
        /// </param>
        public override bool Contains(T item)
        {
            foreach (IList<T> c in _lists)
            {
                if (c.Contains(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="IList{T}"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index.
        /// </summary>
        /// 
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the 
        /// destination of the elements copied from <see cref="IList{T}"/>. 
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
        /// The number of elements in the source <see cref="IList{T}"/> 
        /// is greater than the available space from arrayIndex to the end of 
        /// the destination array. <br/>-or-<br/>
        /// Type T cannot be cast automatically to the type of the destination array.
        /// </exception>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            foreach (IList<T> c in _lists)
            {
                c.CopyTo(array, arrayIndex);
                arrayIndex += c.Count;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IList{T}"/>.
        /// </summary>
        /// <remarks>
        /// This implementation returns total count of all lists that consists
        /// this composite list.
        /// </remarks>
        /// <returns>
        /// The number of elements contained in the <see cref="IList{T}"/>.
        /// </returns>
        /// 
        public override int Count
        {
            get {
                int count = 0;
                foreach (IList<T> c in _lists)
                {
                    count += c.Count;
                }
                return count;
            }
        }

        #endregion

        #region IEnumerable<T> Members

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
            CompositeEnumerator<T> e = new CompositeEnumerator<T>(_lists.Count);
            foreach (IList<T> col in _lists)
            {
                e.Add(col.GetEnumerator());
            }
            return e;
        }

        #endregion

        #region IList Members

        /// <summary>
        /// Copies the elements of the <see cref="System.Collections.IList"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index.
        /// </summary>
        ///
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination 
        /// of the elements copied from <see cref="System.Collections.IList"/>. The 
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
        /// The number of elements in the source <see cref="System.Collections.IList"/> 
        /// is greater than the available space from index to the end of the 
        /// destination array. 
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="System.Collections.IList"/> cannot be cast 
        /// automatically to the type of the destination array. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        protected override void CopyTo(Array array, int index)
        {
            foreach (IList<T> c in _lists)
            {
                foreach (T item in c) array.SetValue(item, index++);
            }
        }

        #endregion

        private static void EnsureNoNull(IEnumerable<IList<T>> e)
        {
            foreach (IList<T> ts in e)
            {
                if (ts==null) throw new NullReferenceException(
                    "List contains null element.");
            }
        }
    }
}
