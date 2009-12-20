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

namespace System.Extension
{
    /// <summary>
    /// Interface for objects that support indexing 
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IIndexable
    {
        /// <summary>
        /// Gets or sets the data with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the data to get or set</param>
        /// <returns>The data with the specified <paramref name="index"/></returns>
        object this[object index] { get;set;}
    }

    /// <summary>
    /// Generic version of <see cref="IIndexable"/>.
    /// </summary>
    /// <typeparam name="TIndex">Type of the index</typeparam>
    /// <typeparam name="TValue">Type of the data value</typeparam>
    public interface IIndexable<TIndex, TValue>
    {
        /// <summary>
        /// Gets or sets the data with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the data to get or set</param>
        /// <returns>The data with the specified <paramref name="index"/></returns>
        TValue this[TIndex index] { get; set;}
    }
}
