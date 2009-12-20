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
    /// <see cref="TransformingDictionary{KFrom,VFrom,KTo,VTo}"/> transforms a
    /// dictionary of (<typeparamref name="KFrom"/>, <typeparamref name="VFrom"/>)
    /// to a dictionary of (<typeparamref name="KTo"/>, <typeparamref name="VTo"/>),
    /// without copying the elements. Elements are transformed on fly when accessed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new dictionary is a shadow copy of the source dictionary.
    /// Changes to any one of the dictionaries will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="TransformingDictionary{TKey,VFrom,VTo}"/> for transforming
    /// dictionary value type only.
    /// </para>
    /// </remarks>
    /// <typeparam name="KFrom">The key type of the source dictionary.</typeparam>
    /// <typeparam name="VFrom">The value type of the source dictionary.</typeparam>
    /// <typeparam name="KTo">The key type of the transformed dictionary.</typeparam>
    /// <typeparam name="VTo">The value type of the transformed dictionary.</typeparam>
    /// <seealso cref="TransformingDictionary{TKey,VFrom,VTo}"/>
    /// <author>Kenneth Xu</author>
    public class TransformingDictionary<KFrom, VFrom, KTo, VTo> : AbstractTransformingDictionary<KFrom, VFrom, KTo, VTo>
    {
        private readonly Converter<KFrom, KTo> _keyTransformer;
        private readonly Converter<KTo, KFrom> _keyReverser;
        private readonly Converter<VFrom, VTo> _valueTransformer;
        private readonly Converter<VTo, VFrom> _valueReverser;

        /// <summary>
        /// Construct a new instance of <see cref="TransformingDictionary{KFrom,VFrom,KTo,VTo}"/>
        /// from given <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        /// The source dictionary to be transformed.
        /// </param>
        /// <param name="keyTransformer">
        /// Delegate to convert source dictionary key type to transformed key type.
        /// </param>
        /// <param name="keyReverser">
        /// Delegate to convert transformed key type to source key type.
        /// </param>
        /// <param name="valueTransformer">
        /// Delegate to convert source dictionary value type to transformed value type.
        /// </param>
        /// <param name="valueReverser">
        /// Delegate to convert transformed value type to source value type.
        /// </param>
        public TransformingDictionary(
            IDictionary<KFrom, VFrom> source,
            Converter<KFrom, KTo> keyTransformer,
            Converter<KTo, KFrom> keyReverser,
            Converter<VFrom, VTo> valueTransformer,
            Converter<VTo, VFrom> valueReverser) 
            : base(source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keyTransformer == null) throw new ArgumentNullException("keyTransformer");
            if (keyReverser == null) throw new ArgumentNullException("keyReverser");
            if (valueTransformer == null) throw new ArgumentNullException("valueTransformer");
            _keyTransformer = keyTransformer;
            _keyReverser = keyReverser;
            _valueTransformer = valueTransformer;
            _valueReverser = valueReverser;
        }

        /// <summary>
        /// Convert dictionary key object of type <typeparamref name="KFrom"/>
        /// to <typeparamref name="KTo"/>.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The converted key.</returns>
        protected override KTo TransformKey(KFrom key)
        {
            return _keyTransformer(key);
        }

        /// <summary>
        /// Reverse convert dictionary key object of type <typeparamref name="KTo"/>
        /// to <typeparamref name="KFrom"/>.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The converted key.</returns>
        protected override KFrom ReverseKey(KTo key)
        {
            return _keyReverser(key);
        }

        /// <summary>
        /// Convert dictionary value object of <typeparamref name="VFrom"/>
        /// to <typeparamref name="VTo"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        protected override VTo TransformValue(VFrom value)
        {
            return _valueTransformer(value);
        }

        /// <summary>
        /// Convert dictionary value object of type <typeparamref name="VTo"/>
        /// to <typeparamref name="VFrom"/>.
        /// </summary>
        /// <remarks>
        /// This implementation always throw <see cref="NotSupportedException"/>.
        /// Derived class should provide implementation if reversing value
        /// type is supported.
        /// </remarks>
        /// <param name="value">Value to be converted.</param>
        /// <returns>
        /// The converted value of type <typeparamref name="VFrom"/>.
        /// </returns>
        protected override VFrom ReverseValue(VTo value)
        {
            return _valueReverser == null ? base.ReverseValue(value) : _valueReverser(value);
        }
    }

    /// <summary>
    /// <see cref="TransformingDictionary{TKey,KTo,VTo}"/> transforms a
    /// dictionary of <typeparamref name="VFrom"/> value type to a dictionary
    /// of <typeparamref name="VTo"/> value type, without copying the elements. 
    /// Elements are transformed on fly when accessed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new dictionary is a shadow copy of the source dictionary.
    /// Changes to any one of the dictionaries will be seen by another.
    /// </para>
    /// <para>
    /// See <see cref="TransformingDictionary{TKey,VFrom,VTo}"/> for transforming
    /// both dictionary key and value type.
    /// </para>
    /// </remarks>
    /// <typeparam name="TKey">
    /// The key type of both source and transformed dictionary.
    /// </typeparam>
    /// <typeparam name="VFrom">The value type of the source dictionary.</typeparam>
    /// <typeparam name="VTo">The value type of the transformed dictionary.</typeparam>
    /// <seealso cref="TransformingDictionary{TKey,VFrom,VTo}"/>
    /// <author>Kenneth Xu</author>
    public class TransformingDictionary<TKey, VFrom, VTo> : AbstractTransformingDictionary<TKey, VFrom, TKey, VTo>
    {
        private readonly Converter<VFrom, VTo> _transformer;
        private readonly Converter<VTo, VFrom> _reverser;

        /// <summary>
        /// Construct a new instance of <see cref="TransformingDictionary{TKey,VFrom,VTo}"/>
        /// from given <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        /// The source dictionary to be transformed.
        /// </param>
        /// <param name="transformer">
        /// Delegate to convert source dictionary value type to transformed value type.
        /// </param>
        /// <param name="reverser">
        /// Delegate to convert transformed value type to source value type.
        /// </param>
        public TransformingDictionary(
            IDictionary<TKey, VFrom> source,
            Converter<VFrom, VTo> transformer,
            Converter<VTo, VFrom> reverser)
            : base(source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (transformer == null) throw new ArgumentNullException("transformer");
            _transformer = transformer;
            _reverser = reverser;
        }

        /// <summary>
        /// This implementation simply return the original key as there is no
        /// convertion required.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The same <paramref name="key"/>.</returns>
        protected sealed override TKey TransformKey(TKey key)
        {
            return key;
        }

        /// <summary>
        /// This implementation simply return the original key as there is no
        /// convertion required.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The same <paramref name="key"/>.</returns>
        protected sealed override TKey ReverseKey(TKey key)
        {
            return key;
        }

        /// <summary>
        /// Convert dictionary value object of <typeparamref name="VFrom"/>
        /// to <typeparamref name="VTo"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        protected sealed override VTo TransformValue(VFrom value)
        {
            return _transformer(value);
        }

        /// <summary>
        /// Convert dictionary value object of type <typeparamref name="VTo"/>
        /// to <typeparamref name="VFrom"/>.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>
        /// The converted value of type <typeparamref name="VFrom"/>.
        /// </returns>
        protected sealed override VFrom ReverseValue(VTo value)
        {
            return _reverser == null ? base.ReverseValue(value) : _reverser(value);
        }
    }
}
