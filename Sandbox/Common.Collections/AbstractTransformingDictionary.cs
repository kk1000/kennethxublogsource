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
    /// Provide basic functions to construct a new strong typed dictionary of
    /// (<typeparamref name="KFrom"/>, <typeparamref name="VFrom"/>) to a 
    /// dictionary of (<typeparamref name="KTo"/>, <typeparamref name="VTo"/>) 
    /// without copying the elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructed new dictionary is a shadow copy of the source dictionary.
    /// Changes to any one of the dictionaries will be seen by another.
    /// </para>
    /// </remarks>
    /// <typeparam name="KFrom">The key type of the source dictionary.</typeparam>
    /// <typeparam name="VFrom">The value type of the source dictionary.</typeparam>
    /// <typeparam name="KTo">The key type of the transformed dictionary.</typeparam>
    /// <typeparam name="VTo">The value type of the transformed dictionary.</typeparam>
    /// <seealso cref="AbstractTransformingCollection{TTo}"/>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractTransformingDictionary<KFrom, VFrom, KTo, VTo> : AbstractDictionary<KTo, VTo>
    {
        /// <summary>
        /// The source dictionary that is being transformed.
        /// </summary>
        protected readonly IDictionary<KFrom, VFrom> _source;

        /// <summary>
        /// Construct the transforming dictionary from given <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        /// The source dictionary to be transformed.
        /// </param>
        protected AbstractTransformingDictionary(IDictionary<KFrom, VFrom> source)
        {
            _source = source;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified 
        /// key, if the key is found; otherwise, the default value for the 
        /// type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the dictionary contains an element with the specified key; 
        /// otherwise, false.
        /// </returns>
        public override bool TryGetValue(KTo key, out VTo value)
        {
            VFrom result;
            var exists = _source.TryGetValue(ReverseKey(key), out result);
            value = exists ? TransformValue(result) : default(VTo);
            return exists;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the key and value pairs
        /// in the dictionary.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<KeyValuePair<KTo, VTo>> GetEnumerator()
        {
            return new TransformingEnumerator<KeyValuePair<KFrom, VFrom>, KeyValuePair<KTo, VTo>>(
                _source.GetEnumerator(),
                p=>new KeyValuePair<KTo, VTo>(TransformKey(p.Key), TransformValue(p.Value)));
        }

        /// <summary>
        /// Adds an element with the provided key and value to the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ///
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The source dictionary is read-only or reverse converting
        /// transformed type to source type is not supported.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override void Add(KTo key, VTo value)
        {
            _source.Add(ReverseKey(key), ReverseValue(value));
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="NotSupportedException">
        /// The source dictionary is read-only or reverse converting
        /// transformed type to source type is not supported.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key is not found.
        /// </exception>
        public override VTo this[KTo key]
        {
            get
            {
                return base[key];
            }
            set
            {
                _source[ReverseKey(key)] = ReverseValue(value);
            }
        }

        /// <summary>
        /// Removes the element with the specified key from the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  
        /// This method also returns false if key was not found in the original 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The source dictionary is read-only or reverse converting
        /// transformed type to source type is not supported.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override bool Remove(KTo key)
        {
            return _source.Remove(ReverseKey(key));
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The source dictionary is read-only.
        /// </exception>
        public override void Clear()
        {
            _source.Clear();
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
        protected virtual VFrom ReverseValue(VTo value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Convert dictionary key object of type <typeparamref name="KFrom"/>
        /// to <typeparamref name="KTo"/>.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The converted key.</returns>
        protected abstract KTo TransformKey(KFrom key);

        /// <summary>
        /// Reverse convert dictionary key object of type <typeparamref name="KTo"/>
        /// to <typeparamref name="KFrom"/>.
        /// </summary>
        /// <param name="key">The key to be converted.</param>
        /// <returns>The converted key.</returns>
        protected abstract KFrom ReverseKey(KTo key);

        /// <summary>
        /// Convert dictionary value object of <typeparamref name="VFrom"/>
        /// to <typeparamref name="VTo"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        protected abstract VTo TransformValue(VFrom value);
    }
}
