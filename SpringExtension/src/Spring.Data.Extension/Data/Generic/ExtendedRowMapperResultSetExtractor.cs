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

#region Imports

using System;
using System.Collections.Generic;
using System.Data;
using Spring.Data.Support;

#endregion

namespace Spring.Data.Generic
{
    /// <summary>
	/// Adapter implementation of the <see cref="IResultSetExtractor"/> interface 
    /// that delegates to a <see cref="IRowMapper"/> or <see cref="RowMapperDelegate"/> 
    /// which are supposed to create an object for each row.  Each object is added 
    /// to the results <see cref="IList{T}"/> of this <see cref="IResultSetExtractor"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Useful for the typical case of one object per row in the database table.
	/// The number of entries in the results list will match the number of rows.
	/// </para>
	/// <para>
	/// Note that a RowMapper object is typically stateless and thus reusable;
    /// just the <see cref="ExtendedRowMapperResultSetExtractor{T}"/> adapter is stateful.
	/// </para>
	/// <para>
    /// As an alternative consider subclassing <see cref="Spring.Data.Objects.MappingAdoQuery"/> 
    /// or its generic version <see cref="Spring.Data.Objects.Generic.MappingAdoQuery"/>:  
    /// Instead of working with separate <see cref="AdoTemplate"/> and <see cref="IRowMapper"/> 
    /// objects you can have executable query objects (containing row-mapping logic) there.
	/// </para>
	/// </remarks>
    /// <author>Mark Pollack (.NET) for RowMapperResultSetExtracto</author>
    /// <author>Kenneth Xu</author>
	public class ExtendedRowMapperResultSetExtractor<T> : DataReaderExtender, IResultSetExtractor<IList<T>> 
	{
		#region Fields

        /// <summary>
        /// An <see cref="IRowMapper{T}"/> to be called for each row.
        /// </summary>
        internal protected IRowMapper<T> _rowMapper;

        internal readonly RowMapperDelegate<T> _rowMapperDelegate;
	    
		#endregion

        #region Constructor (s)

        /// <summary>
        /// Protected constructor to give subclass the flexibility of how row 
        /// mapper is set. It also sets the <see cref="_rowMapper"/> to itself 
        /// if subclass also implements <see cref="IRowMapper{T}"/>.
        /// </summary>
        protected ExtendedRowMapperResultSetExtractor()
        {
            _rowMapper = this as IRowMapper<T>;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedRowMapperResultSetExtractor{T}"/> 
        /// class, which will use the given <paramref name="rowMapper"/> to extract the result.
        /// </summary>
		public ExtendedRowMapperResultSetExtractor(IRowMapper<T> rowMapper)
		{
            if(rowMapper==null) throw new ArgumentNullException("rowMapper");
            _rowMapper = rowMapper;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedRowMapperResultSetExtractor{T}"/> 
        /// class, which will use the given <paramref name="rowMapperDelegate"/> to extract the result.
        /// </summary>
        public ExtendedRowMapperResultSetExtractor(RowMapperDelegate<T> rowMapperDelegate)
        {
            if (rowMapperDelegate == null) throw new ArgumentNullException("rowMapperDelegate");
            _rowMapperDelegate = rowMapperDelegate;
        }

		#endregion

        #region IResultSetExtractor Members

	    ///<summary>
	    ///
	    ///            Implementations must implement this method to process all
	    ///            result set and rows in the IDataReader.
	    ///            
	    ///</summary>
	    ///
	    ///<param name="reader">The IDataReader to extract data from.
	    ///            Implementations should not close this: it will be closed
	    ///            by the AdoTemplate.</param>
	    ///<returns>
	    ///An arbitrary result object or null if none.  The
	    ///            extractor will typically be stateful in the latter case.
	    ///</returns>
	    ///
	    public virtual IList<T> ExtractData(IDataReader reader) 
        {
	        reader = ExtendDataReader(reader);

	        IList<T> results = new List<T>(RowsExpected);
		    int rowNum = 0;
            if (_rowMapper != null)
            {
                while (reader.Read())
                {
                    results.Add(_rowMapper.MapRow(reader, rowNum++));
                }  
            }
            else
            {
                while (reader.Read())
                {
                    results.Add(_rowMapperDelegate(reader, rowNum++));
                }
            }

            return results;
        }

        #endregion
    }
}
