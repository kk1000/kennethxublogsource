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
using System.Extension;

namespace Common.Collection
{
    /// <summary>
    /// A <see cref="ISealableDictionary"/> is a <see cref="IDictionary"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface ISealableDictionary : IDictionary, ISealableCollection, IIndexable
    {
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        ///
        /// <remarks>
        /// Redefined indexer that has same signature as the indexers in both
        /// <see cref="IDictionary"/> and <see cref="IIndexable"/> to avoid 
        /// the ambiguous error in the implementing sub classes.
        /// </remarks>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        ///
        /// <param name="key">
        /// The key of the element to get or set. 
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="IDictionary"/> object 
        /// is read-only.-or- The property is set, key does not exist in 
        /// the collection, and the <see cref="IDictionary"/> has a fixed 
        /// size. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// key is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        new object this[object key] { get; set; }
    }

    /// <summary>
    /// A <see cref="ISealableDictionary{TKey, TValue}"/> is a <see cref="IDictionary{TKey, TValue}"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface ISealableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISealableCollection<KeyValuePair<TKey, TValue>>, IIndexable<TKey, TValue>
    {
        /// <summary>
        /// Redefined indexer that has same signature as the indexers in both
        /// <see cref="IDictionary"/> and <see cref="IIndexable"/> to avoid 
        /// the ambiguous error in the implementing sub classes.
        /// </summary>
        /// <param name="index">The index of the data to get or set</param>
        /// <returns>The data with the specified <paramref name="index"/></returns>
        new TValue this[TKey index] { get; set; }
    }
}
