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
    /// <see cref="DictionaryWrapper{TKey, TValue}"/> is a plain wrapper 
    /// which does nothing but delegate all memeber access to the wrapped 
    /// <see cref="IDictionary{TKey,TValue}"/> instance.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <author>Kenneth Xu</author>
    public class DictionaryWrapper<TKey, TValue> : AbstractDictionaryWrapper<TKey, TValue>
    {
        /// <summary>
        /// Construct a new instance of <see cref="DictionaryWrapper{TKey,TValue}"/>
        /// that wraps the given <paramref name="dictionary"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary to be wrapped.</param>
        /// <exception cref="ArgumentNullException">
        /// When paremeter <paramref name="dictionary"/> is <see langword="null"/>.
        /// </exception>
        public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            WrappedInstance = dictionary;
        }

        /// <summary>
        /// Gets the wrapped instance, which is <see cref="WrappedInstance"/>.
        /// </summary>
        protected override IDictionary<TKey, TValue> Wrapped
        {
            get { return WrappedInstance; }
        }

        /// <summary>
        /// The wrapped dictionary instance.
        /// </summary>
        protected IDictionary<TKey, TValue> WrappedInstance;
    }
}
