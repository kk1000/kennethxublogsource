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
    /// Provide basic functions to construct a new strong typed collection
    /// of type <typeparamref name="TTo"/> from anothe strong typed collection
    /// of type <typeparamref name="TFrom"/> without copying the elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new collection is a shadow copy of the source collection.
    /// Changes to any one of the collections will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="AbstractTransformingCollection{TTo}"/> for transforming
    /// a non-generics collection of type <see cref="ICollection"/> to a strong
    /// typed collection <see cref="ICollection{TTo}">ICollection&lt;TTo></see>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TTo">the element type of the constructed collection</typeparam>
    /// <typeparam name="TFrom">the element type of the source collection</typeparam>
    /// <seealso cref="AbstractTransformingCollection{TTo}"/>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractTransformingCollection<TFrom, TTo> : AbstractCollection<TTo>
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes the <paramref name="source"/> collection.
        /// </summary>
        /// <param name="source">the original source collection</param>
        protected AbstractTransformingCollection(ICollection<TFrom> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            SourceCollection = source;
        }

        #endregion

        #region ICollection<TTo> Members

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>. This implementation
        /// throw <see cref="NotSupportedException"/> if <see cref="TryReverse"/>
        /// returns <see langword="false"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override void Add(TTo item)
        {
            TFrom result;
            if (TryReverse(item, out result)) SourceCollection.Add(result);
            else throw new NotSupportedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>. This implementation
        /// clears the the source collection.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override void Clear()
        {
            SourceCollection.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value. 
        /// </summary>
        /// <remarks>
        /// It <see cref="TryReverse">try reverses</see> the <paramref name="item"/>
        /// and then calls the <see cref="ICollection{T}.Contains"/> method on the source 
        /// collection. Otherwise, it iterates through the enumerator returned by 
        /// <see cref="IEnumerable{T}.GetEnumerator()"/> method of the source collection,
        /// transform each element and compare it with <paramref name="item"/> until a match is
        /// found.
        /// </remarks>
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(TTo item)
        {
            TFrom result;
            if (TryReverse(item, out result)) return SourceCollection.Contains(result);

            foreach (TFrom i in SourceCollection)
            {
                if (Transform(i).Equals(item)) return true;
            }
            return false;
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
            get { return SourceCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// 
        /// <returns>
        /// <see langword="true"/> if the source collection is read only.
        /// </returns>
        /// 
        public override bool IsReadOnly
        {
            get { return SourceCollection.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
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
        /// When the <see cref="ICollection{T}"/> is read-only. See <see cref="IsReadOnly"/>.
        /// </exception>
        public override bool Remove(TTo item)
        {
            TFrom result;
            if (TryReverse(item, out result)) return SourceCollection.Remove(result);

            foreach (TFrom i in SourceCollection)
            {
                if (Transform(i).Equals(item)) return SourceCollection.Remove(i);
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// Subclass must implement this method.
        /// </remarks>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<TTo> GetEnumerator()
        {
            return new TransformingEnumerator<TFrom, TTo>(SourceCollection.GetEnumerator(), Transform);
        }

        #endregion

        #region ICollection Members

        ///<summary>
        ///Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
        ///</summary>
        ///<remarks>This implementaiton always return <see langword="false"/>.</remarks>
        ///<returns>
        ///true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override bool IsSynchronized
        {
            get { return SourceCollection is ICollection && ((ICollection)SourceCollection).IsSynchronized; }
        }

        ///<summary>
        ///Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</summary>
        ///<remarks>This implementation returns <see langword="null"/>.</remarks>
        ///<returns>
        ///An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override object SyncRoot
        {
            get { return SourceCollection is ICollection ? ((ICollection)SourceCollection).SyncRoot : null; }
        }

        #endregion

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

        /// <summary>
        /// Try converts object of type <typeparamref name="TTo"/> to
        /// <typeparamref name="TFrom"/>.
        /// </summary>
        /// <remarks>
        /// This implementation always return <c>false</c>. Subclasses that
        /// support reversing should override this method.
        /// </remarks>
        /// <param name="target">
        /// Instance of <typeparamref name="TTo"/> to be converted.
        /// </param>
        /// <param name="source">
        /// Converted object of type <typeparamref name="TFrom"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> when reserving is supported, otherwise <c>false</c>.
        /// </returns>
        protected virtual bool TryReverse(TTo target, out TFrom source)
        {
            source = default(TFrom);
            return false;
        }

        /// <summary>
        /// The base collection that is being transformed.
        /// </summary>
        protected ICollection<TFrom> SourceCollection;

    }

    /// <summary>
    /// Provide basic functions to construct a new strong typed collection
    /// of type <typeparamref name="TTo"/> from a non-generics collection
    /// <see cref="ICollection"/> without copying the elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new collection is a shadow copy of the source collection.
    /// Changes to any one of the collections will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="AbstractTransformingCollection{TFrom, TTo}"/> for 
    /// transforming one generics collection of <c>TFrom</c> to another strong
    /// typed collection of <c>TTo</c>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TTo">the element type of the constructed collection</typeparam>
    /// <seealso cref="AbstractTransformingCollection{TFrom,TTo}"/>
    public abstract class AbstractTransformingCollection<TTo> : AbstractCollection<TTo>, ICollection<TTo>, ICollection
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes the <paramref name="source"/> collection.
        /// </summary>
        /// <param name="source">the original source collection</param>
        protected AbstractTransformingCollection(ICollection source)
        {
            if (source == null) throw new ArgumentNullException("source");
            _c = source;
        }

        #endregion

        #region ICollection<TTo> Members

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value. This implementation searchs the element by comparing to each
        /// transformed element of source collection.
        /// </summary>
        /// 
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(TTo item)
        {
            foreach (object o in _c)
            {
                if (Equals(Transform(o),item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// This implementation returns the count of source collection.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        public override int Count
        {
            get { return _c.Count; }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// Subclass must implement this method.
        /// </remarks>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<TTo> GetEnumerator()
        {
            return new TransformingEnumerator<TTo>(_c.GetEnumerator(), Transform);
        }

        #endregion

        #region ICollection Members

        ///<summary>
        ///Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
        ///</summary>
        ///<remarks>This implementaiton always return <see langword="false"/>.</remarks>
        ///<returns>
        ///true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override bool IsSynchronized
        {
            get { return _c.IsSynchronized; }
        }

        ///<summary>
        ///Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</summary>
        ///<remarks>This implementation returns <see langword="null"/>.</remarks>
        ///<returns>
        ///An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        protected override object SyncRoot
        {
            get { return _c.SyncRoot; }
        }

        #endregion

        /// <summary>
        /// Converts object in the source collection to
        /// <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="source">
        /// element in the source collection.
        /// </param>
        /// <returns>
        /// Converted object of type <typeparamref name="TTo"/>.
        /// </returns>
        protected abstract TTo Transform(object source);

        /// <summary>
        /// The base collection that is being transformed.
        /// </summary>
        protected ICollection _c;

    }

}
