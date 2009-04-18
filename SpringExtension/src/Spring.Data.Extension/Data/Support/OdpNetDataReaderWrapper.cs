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
using Oracle.DataAccess.Types;

namespace Spring.Data.Support
{
    /// <summary>
    /// An <see cref="IDataReader"/> that wrappes the ODP.Net data reader
    /// to provide ODP.Net not implemented getters.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class OdpNetDataReaderWrapper : DataReaderWrapperBase
    {
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
    }
}