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
using System.Data;
using System.Reflection;
using Common.Logging;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Spring.Data.Support
{
    /// <summary>
    /// An <see cref="IDataReader"/> that wrappes the ODP.Net data reader
    /// to provide ODP.Net not implemented getters.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class OdpNetDataReaderWrapper : ExtendedDataReaderWrapper
    {
        /// <summary>
        /// The default maximal fetch buffer size.
        /// </summary>
        public const int DEFAULT_MAX_FETCH_SIZE = 8 * 1024 * 1024; //8M

        private static readonly ILog _log = LogManager.GetLogger(typeof(OdpNetDataReaderWrapper));

        static internal readonly FieldInfo RowSizeFieldInfo;

        private static int _maxFetchSize = DEFAULT_MAX_FETCH_SIZE;

        static volatile bool _isWrongWrappedReaderTypeWarningGiven;

        private OracleDataReader oracleReader;

        static OdpNetDataReaderWrapper()
        {
            RowSizeFieldInfo = typeof(OracleDataReader).GetField("m_rowSize", BindingFlags.Instance | BindingFlags.NonPublic);
            if(RowSizeFieldInfo == null && _log.IsWarnEnabled)
            {
                _log.Warn("Unsupported Odp.Net version :" + typeof(OracleDataReader).Assembly.FullName + 
                    ". Dynamic fetch size is off, using default fetch size of connection.");
            }
        }

        /// <summary>
        /// Gets and sets the maximal fetch buffer size used to constraint the 
        /// actual <see cref="OracleDataReader.FetchSize"/> calculated from 
        /// <see cref="RowsExpected"/> and <see cref="RowSize"/>.
        /// </summary>
        public static int MaxFetchSize
        {
            get { return _maxFetchSize; }
            set { _maxFetchSize = value; }
        }

        /// <summary>
        /// Provide the implementation for <see cref="IDataRecord.GetBoolean"/>.
        /// by assuming 
        /// <c>false</c>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// <para>
        /// When the underlaying data column is a character string, a single
        /// character 'Y' indicates <c>true</c> and anything else indicates
        /// <c>false</c>.
        /// </para>
        /// <para>
        /// When the underlaying data column is number type, zero value or 
        /// indicates <c>false</c>, any other value indicates <c>true</c>.
        /// </para>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through 
        /// <see cref="IDataRecord.FieldCount" />. 
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Field data type is neither character types nor number types.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override bool GetBoolean(int i)
        {
            switch (dataReader.GetDataTypeName(i))
            {
                case "Char":
                case "Varchar2":
                case "NChar":
                case "NVarchar2":
                    string s = dataReader.GetString(i);
                    return s.Length == 1 && s[0] == 'Y';
                case "Int16":
                    return dataReader.GetInt16(i) != 0;
                case "Int32":
                    return dataReader.GetInt32(i) != 0;
                case "Int64":
                    return dataReader.GetInt64(i) != 0;
                case "Decimal":
                    return dataReader.GetDecimal(i) != 0;
                case "Single":
                    return dataReader.GetFloat(i) != 0;
                case "Double":
                    return dataReader.GetDouble(i) != 0;
                default:
                    throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Provide the implementation for <see cref="IDataRecord.GetChar"/>.
        /// to get value from Oracle CHAR(1) column type.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through 
        /// <see cref="IDataRecord.FieldCount" />. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override char GetChar(int i)
        {
            string value = dataReader.GetString(i);
            if (value.Length != 1) throw new OracleTruncateException(
                "Expecting exactly one character but got '" + value + "'.");
            return value[0];
        }

        /// <summary>
        /// The underlying reader implementation to delegate to for accessing 
        /// data from a returned result sets.
        /// </summary>
        /// <remarks>
        /// When this properties is set, it calls the setter in the base class
        /// first, then set the fetch size of the reader.
        /// </remarks>
        /// <value>
        /// The wrapped reader.
        /// </value>
        public override IDataReader WrappedReader
        {
            get
            {
                return base.WrappedReader;
            }
            set
            {
                base.WrappedReader = value;
                SetFetchSize();
            }
        }

        /// <summary>
        /// Gets and sets the number of rows to expect from the 
        /// <see cref="WrappedReader"/>. When the <c>WrappedReader</c> is also 
        /// an <see cref="ExtendedDataReaderWrapper"/> and this property is
        /// set, it will be further propagate into the <c>WrappedReader</c>.
        /// </summary>
        /// <remarks>
        /// When this properties is set, it calls the setter in the base class
        /// first, then set the fetch size of the wrapped reader.
        /// </remarks>
        public override int RowsExpected
        {
            get
            {
                return base.RowsExpected;
            }
            set
            {
                base.RowsExpected = value;
                SetFetchSize();
            }
        }

        private void SetFetchSize()
        {
            int rowsExpected = RowsExpected;
            if (rowsExpected > 0 && RowSizeFieldInfo != null)
            {
                var unwrappedReader = GetInnerMostReader();
                oracleReader =  unwrappedReader as OracleDataReader;
                if (oracleReader != null)
                {
                    int rowSize = RowSize;
                    if (rowSize>0)
                    {
                        int fetchSize = rowSize * rowsExpected;
                        int maxSize = MaxFetchSize;
                        if (fetchSize > maxSize)
                        {
                            int ratio = (fetchSize - 1)/maxSize + 1;
                            fetchSize = (fetchSize - 1) / ratio + 1;
                        }
                        oracleReader.FetchSize = fetchSize;
                    }
                }
                else if(!_isWrongWrappedReaderTypeWarningGiven && _log.IsWarnEnabled)
                {
                    _log.Warn(String.Format("Expected original reader to be {0} but got {1}. Note: warning is suspended for subsequent repeated events.", 
                        typeof(OracleDataReader).FullName, unwrappedReader.GetType().FullName));
                    _isWrongWrappedReaderTypeWarningGiven = true;
                }
            }
        }

        /// <summary>
        /// This is to facilitate the unit test.
        /// </summary>
        protected internal virtual int RowSize
        {
            get
            {
                return (int) RowSizeFieldInfo.GetValue(oracleReader);
            }
        }
    }
}