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
    /// Create an enumerator that can enumerate through multiple enumerators. 
    /// Enumerators can be added to the composition using the provided methods.
    /// </summary>
    /// <typeparam name="T">The type of the elements to enumerate through</typeparam>
    /// <seealso cref="SimpleCompositeEnumerator{T}"/>
    /// <author>Kenneth Xu</author>
    public class CompositeEnumerator<T> : AbstractCompositeEnumerator<T>
    {
        private readonly List<IEnumerator<T>> _enumerators;

        #region Operators
        
        /// <summary>
        /// Add enumerator to a <see cref="CompositeEnumerator{T}"/>.
        /// </summary>
        /// <param name="ce">A <see cref="CompositeEnumerator{T}"/>.</param>
        /// <param name="e">An enumerator to be added.</param>
        /// <returns>The same <see cref="CompositeEnumerator{T}"/> <paramref name="ce"/></returns>
        public static CompositeEnumerator<T> operator +(CompositeEnumerator<T> ce, IEnumerator<T> e)
        {
            ce.Add(e);
            return ce;
        }

        /// <summary>
        /// Insert enumerator to a <see cref="CompositeEnumerator{T}"/>.
        /// </summary>
        /// <param name="ce">A <see cref="CompositeEnumerator{T}"/>.</param>
        /// <param name="e">An enumerator to be inserted.</param>
        /// <returns>The same <see cref="CompositeEnumerator{T}"/> <paramref name="ce"/></returns>
        public static CompositeEnumerator<T> operator +(IEnumerator<T> e, CompositeEnumerator<T> ce)
        {
            ce.Insert(0, e);
            return ce;
        }

        /// <summary>
        /// Add a range of enumerators to a <see cref="CompositeEnumerator{T}"/>.
        /// </summary>
        /// <param name="ce">A <see cref="CompositeEnumerator{T}"/>.</param>
        /// <param name="es">A range of enumerators to be added.</param>
        /// <returns>The same <see cref="CompositeEnumerator{T}"/> <paramref name="ce"/></returns>
        public static CompositeEnumerator<T> operator +(CompositeEnumerator<T> ce, IEnumerable<IEnumerator<T>> es)
        {
            ce.AddRange(es);
            return ce;
        }

        /// <summary>
        /// Insert a range of enumerators to a <see cref="CompositeEnumerator{T}"/>.
        /// </summary>
        /// <param name="ce">A <see cref="CompositeEnumerator{T}"/>.</param>
        /// <param name="es">A range of enumerators to be inserted.</param>
        /// <returns>The same <see cref="CompositeEnumerator{T}"/> <paramref name="ce"/></returns>
        public static CompositeEnumerator<T> operator +(IEnumerable<IEnumerator<T>> es, CompositeEnumerator<T> ce)
        {
            ce.InsertRange(0, es);
            return ce;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct an empty enumerator that other enumerator components can
        /// be added later.
        /// </summary>
        public CompositeEnumerator()
            : this(null, 0)
        {
        }

        /// <summary>
        /// Construct an enumerator with the given initial <paramref name="capacity"/>
        /// that other enumerator components can be added later.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeEnumerator(int capacity)
            : this(null, capacity)
        {
        }

        /// <summary>
        /// Construct an enumerator that iterator though multiple 
        /// <paramref name="enumerators"/>. More enumerators can be added later.
        /// </summary>
        /// <param name="enumerators">
        /// Enuemrators this <see cref="CompositeEnumerator{T}"/> initially contains.
        /// </param>
        public CompositeEnumerator(IEnumerable<IEnumerator<T>> enumerators)
            : this(enumerators, 0)
        {
        }

        /// <summary>
        /// Construct an enumerator with the given initial <paramref name="capacity"/> 
        /// that iterator though multiple <paramref name="enumerators"/>. More 
        /// enumerators can be added later.
        /// </summary>
        /// <param name="enumerators">
        /// Enuemrators this <see cref="CompositeEnumerator{T}"/> initially contains.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeEnumerator(IEnumerable<IEnumerator<T>> enumerators, int capacity)
        {
            _enumerators = capacity > 0 ?
                new List<IEnumerator<T>>(capacity) :
                new List<IEnumerator<T>>();
            if (enumerators != null) _enumerators.AddRange(enumerators);
        }

        #endregion

        /// <summary>
        /// Gets the enumerator to iterate through all compoisted enumerators.
        /// </summary>
        protected override IEnumerable<IEnumerator<T>> Enumerators
        {
            get { return _enumerators; }
        }

        /// <summary>
        /// Add the <paramref name="enumerator"/> to the end of composition.
        /// </summary>
        /// <param name="enumerator">
        /// The enumerator to be added to the end of the composition.
        /// </param>
        public void Add(IEnumerator<T> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException("enumerator");
            _enumerators.Add(enumerator);
        }

        /// <summary>
        /// Insert the <paramref name="enumerator"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="enumerator">The enumerator to be inserted.</param>
        public void Insert(int position, IEnumerator<T> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException("enumerator");
            _enumerators.Insert(position, enumerator);
        }

        /// <summary>
        /// Add a range of <paramref name="enumerators"/> to the end of the composition.
        /// </summary>
        /// <param name="enumerators">
        /// The enumerators to be added to the end of the composition.
        /// </param>
        public void AddRange(IEnumerable<IEnumerator<T>> enumerators)
        {
            if (enumerators == null) throw new ArgumentNullException("enumerators");
            _enumerators.AddRange(enumerators);
        }

        /// <summary>
        /// Insert a range of <paramref name="enumerators"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="enumerators">The enumerator to be inserted.</param>
        public void InsertRange(int position, IEnumerable<IEnumerator<T>> enumerators)
        {
            if (enumerators == null) throw new ArgumentNullException("enumerators");
            _enumerators.InsertRange(position, enumerators);
        }
    }
}
