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

using System;
using System.Collections.Generic;
using System.Data;

namespace Spring.Data.Support
{
    /// <summary>
    /// Thread safe implementation of <see cref="IDataRecordOrdinalCache"/> 
    /// that enables fast, case insensitive, lookup for to ordinal of given 
    /// field names in <see cref="IDataRecord"/> objects.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class DataRecordOrdinalCache : IDataRecordOrdinalCache
    {
        private IDictionary<string, int> _ordinalMap;
        private volatile bool _isInitCompleted;

        #region IDataRecordOrdinalCache Members

        /// <summary>
        /// Initialize the cache. Must be called at least once before any other
        /// method can be called.
        /// </summary>
        /// <remarks>
        /// First call will build the cache. Subsequent calls are ignored.
        /// Please be aware that the subsequent call doesn't check against a
        /// <see langword="volatile"/> field, so avoid calling this method
        /// for every row in an instance of <see cref="IDataRecord"/>.
        /// </remarks>
        /// <param name="dataRecord">
        /// The <see cref="IDataRecord"/> object used to initialize the cache.
        /// </param>
        public void Init(IDataRecord dataRecord)
        {
            if (dataRecord == null) throw new ArgumentNullException("dataRecord");
            if (!_isInitCompleted)
            {
                lock (this)
                {
                    _ordinalMap = BuildOrdinalMap(dataRecord);
                    _isInitCompleted = true;
                }
            }
        }

        /// <summary>
        /// Get the ordinal of field with given <paramref name="name"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation is case insensitive. field names with all lower 
        /// case, all upper case, or exact case as returned from the
        /// <see cref="IDataRecord.GetName"/> method will yield fastest lookup 
        /// speed. Otherwise, second attemp will be made by converting the 
        /// <paramref name="name"/> to upper case.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the field.</param>
        /// <returns>The cached ordinal of the field.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When cannot find the <paramref name="name"/> in cache.
        /// </exception>
        public int GetOrdinal(string name)
        {
            if (_ordinalMap == null)
            {
                throw new InvalidOperationException("Not yet initialized. Call Init method first.");
            }
            int index;
            // try exact match which is fastest
            if (_ordinalMap.TryGetValue(name, out index)) return index;
            // try upper case match
            if (_ordinalMap.TryGetValue(name.ToUpper(), out index)) return index;

            throw new IndexOutOfRangeException(name + " field doesn't exist in the cache." + 
                " Are you using the cache to an IDataRecord that has difference set of fields?");
        }

        #endregion

        #region Private Static Methods

        private static IDictionary<string, int> BuildOrdinalMap(IDataRecord record)
        {
            int count = record.FieldCount;
            IDictionary<string, int> map = new Dictionary<string, int>(count * 3);
            for (int i = 0; i < count; i++)
            {
                string name = record.GetName(i);
                map[name] = i;
                map[name.ToLower()] = i;
                map[name.ToUpper()] = i;
            }
            return map;
        }

        #endregion

    }
}