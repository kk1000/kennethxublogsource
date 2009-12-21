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
    /// <see cref="TransformingCollection{TFrom, TTo}"/> transforms a
    /// collection of type <typeparamref name="TFrom"/> to a collection of type 
    /// <typeparamref name="TTo"/> without copying the elements. Elements are
    /// transformed on fly when accessed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new collection is a shadow copy of the source collection.
    /// Changes to any one of the collections will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="TransformingCollection{TTo}"/> for transforming
    /// a non-generics collection of type <see cref="ICollection"/> to a strong
    /// typed collection <see cref="ICollection{TTo}">ICollection&lt;TTo></see>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TTo">the element type of the constructed collection</typeparam>
    /// <typeparam name="TFrom">the element type of the source collection</typeparam>
    /// <seealso cref="TransformingCollection{TTo}"/>
    /// <author>Kenneth Xu</author>
    public class TransformingCollection<TFrom, TTo> : AbstractTransformingCollection<TFrom, TTo>
    {
        #region Constructors

        /// <summary>
        /// Construct a new instance of <see cref="TransformingCollection{TFrom,TTo}"/>
        /// that transforming the elements from <paramref name="source"/> using the 
        /// <paramref name="transformer"/>.
        /// </summary>
        /// <param name="source">The source collection to be transformed.</param>
        /// <param name="transformer">
        /// The transformer to transform the element in source collection to type 
        /// <typeparamref name="TTo"/>.
        /// </param>
        public TransformingCollection(ICollection<TFrom> source, Converter<TFrom, TTo> transformer) 
            : this(source, transformer, null)
        {
        }

        /// <summary>
        /// Construct a new instance of <see cref="TransformingCollection{TFrom,TTo}"/>
        /// that transforming the elements from <paramref name="source"/> using the
        /// <paramref name="transformer"/> and <paramref name="reverser"/> to convert
        /// the element on fly.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="transformer"></param>
        /// <param name="reverser"></param>
        public TransformingCollection(ICollection<TFrom> source, Converter<TFrom, TTo> transformer, 
            Converter<TTo, TFrom> reverser) : base(source)
        {
            if (transformer == null) throw new ArgumentNullException("transformer");
            _transformer = transformer;
            _reverser = reverser;
        }

        #endregion

        /// <summary>
        /// Converts object of type <typeparamref name="TFrom"/> to
        /// <typeparamref name="TTo"/> using the transformer passed into 
        /// the constructor.
        /// </summary>
        /// <param name="source">
        /// Instace of <typeparamref name="TFrom"/> to be converted.
        /// </param>
        /// <returns>
        /// Converted object of type <typeparamref name="TTo"/>.
        /// </returns>
        protected override TTo Transform(TFrom source)
        {
            return _transformer(source);
        }

        /// <summary>
        /// Converts object of type <typeparamref name="TTo"/> to
        /// <typeparamref name="TFrom"/>.
        /// </summary>
        /// <param name="target">
        /// Instance of <typeparamref name="TTo"/> to be converted.
        /// </param>
        /// <param name="source">
        /// Converted object of type <typeparamref name="TFrom"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> when a reverser was given when the instance was 
        /// constructed, otherwise <c>false</c>.
        /// </returns>
        protected override bool TryReverse(TTo target, out TFrom source)
        {
            if (_reverser==null)
            {
                source = default(TFrom);
                return false;
            } 
            else
            {
                source = _reverser(target);
                return true;
            }
        }

        #region Private Instance Fields
        private readonly Converter<TFrom, TTo> _transformer;
        private readonly Converter<TTo, TFrom> _reverser;
        #endregion
    }

    /// <summary>
    /// <see cref="TransformingCollection{TTo}"/> transforms a non-generics 
    /// <see cref="ICollection"/> to a collection of type 
    /// <typeparamref name="TTo"/> without copying the elements. Elements are
    /// transformed on fly when accessed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new collection is a shadow copy of the source collection.
    /// Changes to any one of the collections will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="TransformingCollection{From, TTo}"/> for transforming
    /// a generics collection of type <see cref="ICollection{TFrom}">ICollection&lt;TFrom></see> 
    /// to a strong typed collection <see cref="ICollection{TTo}">ICollection&lt;TTo></see>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TTo">the element type of the constructed collection</typeparam>
    /// <seealso cref="TransformingCollection{TFrom, TTo}"/>
    public class TransformingCollection<TTo> : AbstractTransformingCollection<TTo>
    {
        #region Constructors

        /// <summary>
        /// Construct a new instance of <see cref="TransformingCollection{TFrom,TTo}"/>
        /// that transforming the elements from <paramref name="source"/> using the 
        /// <paramref name="transformer"/>.
        /// </summary>
        /// <param name="source">The source collection to be transformed.</param>
        /// <param name="transformer">
        /// The transformer to transform the element in source collection to type 
        /// <typeparamref name="TTo"/>.
        /// </param>
        public TransformingCollection(ICollection source, Converter<object, TTo> transformer)
            : base(source)
        {
            if (transformer == null) throw new ArgumentNullException("transformer");
            _transformer = transformer;
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
        protected override TTo Transform(object source)
        {
            return _transformer(source);
        }

        #region Private Instance Fields
        private readonly Converter<object, TTo> _transformer;
        #endregion
    }
}
