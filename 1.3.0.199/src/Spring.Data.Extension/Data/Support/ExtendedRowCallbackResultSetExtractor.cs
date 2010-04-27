#region License

/*
 * Copyright 2002-2007 the original author or authors.
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
using Spring.Util;

namespace Spring.Data.Support
{
    /// <summary>
    /// Adapter to enable use of an <see cref="IRowCallback"/> or a 
    /// <see cref="_rowCallbackDelegate"/> inside an 
    /// <see cref="IResultSetExtractor"/>.
    /// </summary>
    /// <remarks>
    /// We don't use it for navigating since this could lead to unpredictable consequences.
    /// </remarks>
    /// <author>Mark Pollack for RowCallbackResultSetExtractor</author>
    /// <author>Kenneth Xu</author>
    public class ExtendedRowCallbackResultSetExtractor : DataReaderExtender, IResultSetExtractor, Spring.Data.Generic.IResultSetExtractor<object>
    {
        /// <summary>
        /// An <see cref="IRowCallback"/> to be called for each row.
        /// </summary>
        internal protected IRowCallback _rowCallback;

        internal readonly RowCallbackDelegate _rowCallbackDelegate;

        /// <summary>
        /// Protected constructor to give subclass the flexibility of how
        /// callback is set. It also sets the <see cref="_rowCallback"/> to 
        /// itself if subclass also implements <see cref="IRowCallback"/>.
        /// </summary>
        protected ExtendedRowCallbackResultSetExtractor()
        {
            _rowCallback = this as IRowCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedRowCallbackResultSetExtractor"/> 
        /// class to extract result using given <paramref name="rowCallback"/>.
        /// </summary>
        /// <param name="rowCallback">The row callback.</param>
        public ExtendedRowCallbackResultSetExtractor(IRowCallback rowCallback)
        {
            AssertUtils.ArgumentNotNull(rowCallback, "rowCallback");
            this._rowCallback = rowCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedRowCallbackResultSetExtractor"/>
        /// class to extract result using given <paramref name="rowCallbackDelegate"/>.
        /// </summary>
        /// <param name="rowCallbackDelegate">The row callback delegate.</param>
        public ExtendedRowCallbackResultSetExtractor(RowCallbackDelegate rowCallbackDelegate)
        {
            AssertUtils.ArgumentNotNull(rowCallbackDelegate, "rowCallbackDelegate");
            this._rowCallbackDelegate = rowCallbackDelegate;
        }

        /// <summary>
        /// All rows of the data reader are passed to the <see cref="IRowCallback"/>
        /// or <see cref="RowCallbackDelegate"/> associated with this class.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="IDataReader"/> to extract data from.
        /// Implementations should not close this: it will be closed
        /// by the AdoTemplate.</param>
        /// <returns>
        /// Null is returned since <see cref="IRowCallback"/> or 
        /// <see cref="RowCallbackDelegate"/> manages its own state.
        /// </returns>
        public virtual object ExtractData(IDataReader reader)
        {
            reader = ExtendDataReader(reader);
            if (_rowCallback != null)
            {
                while (reader.Read())
                {
                    _rowCallback.ProcessRow(reader);
                }
            }
            else
            {
                while (reader.Read())
                {
                    _rowCallbackDelegate(reader);
                }
            }

            return null;
        }
    }
}