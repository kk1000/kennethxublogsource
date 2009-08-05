#region License

/*
 * Copyright ?2002-2006 the original author or authors.
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
using Spring.Data.Support;

namespace Spring.Data.Support
{
    /// <summary>
    /// <see cref="DataReaderExtender"/> extends existing data
    /// </summary>
    public class DataReaderExtender
    {
        private int _rowsExpected;
        private IDataRecordOrdinalCache _ordinalCache;

        /// <summary>
        /// Gets and sets rows expected to return.
        /// </summary>
        public virtual int RowsExpected
        {
            get { return _rowsExpected; }
            set { _rowsExpected = value; }
        }

        /// <summary>
        /// Gets and sets the ordinal cache.
        /// </summary>
        public virtual IDataRecordOrdinalCache OrdinalCache
        {
            get { return _ordinalCache; }
            set { _ordinalCache = value; }
        }

        internal IDataReader ExtendDataReader(IDataReader reader)
        {
            if (_ordinalCache != null || _rowsExpected > 0)
            {
                var wrapper = reader as ExtendedDataReaderWrapper;
                if (_ordinalCache != null)
                {
                    if (wrapper == null)
                    {
                        wrapper = new ExtendedDataReaderWrapper { WrappedReader = reader };
                    }
                    wrapper.OrdinalCache = _ordinalCache;
                }
                if (_rowsExpected > 0 && wrapper != null)
                {
                    wrapper.RowsExpected = _rowsExpected;
                }
                return wrapper ?? reader;
            }
            return reader;
        }
    }
}