#region License

/*
 * Copyright (C) 2009 the original author or authors.
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

using System.Data;

namespace Spring.Data
{
    /// <summary>
    /// Defines the operation of field name to ordinal cache to improve the
    /// performance of retrieve data from <see cref="IDataRecord"/> by field
    /// name. Implementation must be thread safe.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IDataRecordOrdinalCache
    {
        /// <summary>
        /// Initialize the cache. Must be called at least once before any other
        /// method can be called.
        /// </summary>
        /// <remarks>
        /// First call will build the cache. Subsequent calls are ignored.
        /// </remarks>
        /// <param name="dataRecord">
        /// The <see cref="IDataRecord"/> object used to initialize the cache.
        /// </param>
        void Init(IDataRecord dataRecord);

        /// <summary>
        /// Get the ordinal of field with given <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>The cached ordinal of the field.</returns>
        int GetOrdinal(string name);
    }
}
