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

namespace Spring.Data.Support
{
    /// <summary>
    /// This class delegates all calls to the wrapped data reader. It serves
    /// as a convenient base class for various implementaiton of
    /// <see cref="IDataReaderWrapper"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class DataReaderWrapperBase : IDataReaderWrapper
    {
        private bool alreadyDisposed;
        
        /// <summary>
        /// The wrapped data reader.
        /// </summary>
        protected IDataReader dataReader;
         
        ///<summary>
        ///
        ///                    Gets a value indicating the depth of nesting for the current row.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The level of nesting.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public virtual int Depth
        {
            get
            {
                return this.dataReader.Depth;
            }
        }

        ///<summary>
        ///
        ///                    Gets the number of columns in the current row.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public virtual int FieldCount
        {
            get
            {
                return this.dataReader.FieldCount;
            }
        }

        ///<summary>
        ///
        ///                    Gets a value indicating whether the data reader is closed.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if the data reader is closed; otherwise, false.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public virtual bool IsClosed
        {
            get
            {
                return this.dataReader.IsClosed;
            }
        }

        ///<summary>
        ///
        ///                    Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public virtual int RecordsAffected
        {
            get
            {
                return this.dataReader.RecordsAffected;
            }
        }

        ///<summary>
        ///
        ///            The underlying reader implementation to delegate to for accessing data 
        ///            from a returned result sets.
        ///            
        ///</summary>
        ///
        ///<value>
        ///The wrapped reader.
        ///</value>
        ///
        public virtual IDataReader WrappedReader
        {
            get
            {
                return this.dataReader;
            }
            set
            {
                this.dataReader = value;
            }
        }

        ///<summary>
        ///
        ///                    Closes the <see cref="T:System.Data.IDataReader" /> Object.
        ///                
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public virtual void Close()
        {
            this.dataReader.Close();
        }

        ///<summary>
        ///
        ///                    Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///                
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        /// <summary>
        /// For sub class to override and performs tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">
        /// To indicate if the method is called from <see cref="Dispose()"/>
        /// method or the Finallizer.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!this.alreadyDisposed)
            {
                if (isDisposing)
                {
                    this.dataReader.Dispose();
                }
                this.alreadyDisposed = true;
            }
        }

        ///<summary>
        ///
        ///                    Allows an <see cref="T:System.Object" /> to attempt to free resources and perform other cleanup operations before the <see cref="T:System.Object" /> is reclaimed by garbage collection.
        ///                
        ///</summary>
        ///
        ~DataReaderWrapperBase()
        {
            this.Dispose(false);
        }

        ///<summary>
        ///
        ///                    Gets the value of the specified column as a Boolean.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The value of the column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual bool GetBoolean(int i)
        {
            return dataReader.GetBoolean(i);
        }

        ///<summary>
        ///
        ///                    Gets the 8-bit unsigned integer value of the specified column.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 8-bit unsigned integer value of the specified column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual byte GetByte(int i)
        {
            return dataReader.GetByte(i);
        }

        ///<summary>
        ///
        ///                    Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The actual number of bytes read.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<param name="fieldOffset">
        ///                    The index within the field from which to start the read operation. 
        ///                </param>
        ///<param name="buffer">
        ///                    The buffer into which to read the stream of bytes. 
        ///                </param>
        ///<param name="bufferoffset">
        ///                    The index for <paramref name="buffer" /> to start the read operation. 
        ///                </param>
        ///<param name="length">
        ///                    The number of bytes to read. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        ///<summary>
        ///
        ///                    Gets the character value of the specified column.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The character value of the specified column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual char GetChar(int i)
        {
            return dataReader.GetChar(i);
        }

        ///<summary>
        ///
        ///                    Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The actual number of characters read.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<param name="fieldoffset">
        ///                    The index within the row from which to start the read operation. 
        ///                </param>
        ///<param name="buffer">
        ///                    The buffer into which to read the stream of bytes. 
        ///                </param>
        ///<param name="bufferoffset">
        ///                    The index for <paramref name="buffer" /> to start the read operation. 
        ///                </param>
        ///<param name="length">
        ///                    The number of bytes to read. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        ///<summary>
        ///
        ///                    Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    An <see cref="T:System.Data.IDataReader" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual IDataReader GetData(int i)
        {
            return this.dataReader.GetData(i);
        }

        ///<summary>
        ///
        ///                    Gets the data type information for the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The data type information for the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual string GetDataTypeName(int i)
        {
            return this.dataReader.GetDataTypeName(i);
        }

        ///<summary>
        ///
        ///                    Gets the date and time data value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The date and time data value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual DateTime GetDateTime(int i)
        {
            return this.dataReader.GetDateTime(i);
        }

        ///<summary>
        ///
        ///                    Gets the fixed-position numeric value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The fixed-position numeric value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual decimal GetDecimal(int i)
        {
            return this.dataReader.GetDecimal(i);
        }

        ///<summary>
        ///
        ///                    Gets the double-precision floating point number of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The double-precision floating point number of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual double GetDouble(int i)
        {
            return this.dataReader.GetDouble(i);
        }

        ///<summary>
        ///
        ///                    Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual Type GetFieldType(int i)
        {
            return this.dataReader.GetFieldType(i);
        }

        ///<summary>
        ///
        ///                    Gets the single-precision floating point number of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The single-precision floating point number of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual float GetFloat(int i)
        {
            return this.dataReader.GetFloat(i);
        }

        ///<summary>
        ///
        ///                    Returns the GUID value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The GUID value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual Guid GetGuid(int i)
        {
            return this.dataReader.GetGuid(i);
        }

        ///<summary>
        ///
        ///                    Gets the 16-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 16-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual short GetInt16(int i)
        {
            return this.dataReader.GetInt16(i);
        }

        ///<summary>
        ///
        ///                    Gets the 32-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 32-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual int GetInt32(int i)
        {
            return this.dataReader.GetInt32(i);
        }

        ///<summary>
        ///
        ///                    Gets the 64-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 64-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual long GetInt64(int i)
        {
            return this.dataReader.GetInt64(i);
        }

        ///<summary>
        ///
        ///                    Gets the name for the field to find.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The name of the field or the empty string (""), if there is no value to return.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual string GetName(int i)
        {
            return this.dataReader.GetName(i);
        }

        ///<summary>
        ///
        ///                    Return the index of the named field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The index of the named field.
        ///                
        ///</returns>
        ///
        ///<param name="name">
        ///                    The name of the field to find. 
        ///                </param><filterpriority>2</filterpriority>
        public virtual int GetOrdinal(string name)
        {
            return this.dataReader.GetOrdinal(name);
        }

        ///<summary>
        ///
        ///                    Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        ///                
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        ///                    The <see cref="T:System.Data.IDataReader" /> is closed. 
        ///                </exception><filterpriority>2</filterpriority>
        public DataTable GetSchemaTable()
        {
            return this.dataReader.GetSchemaTable();
        }

        ///<summary>
        ///
        ///                    Gets the string value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The string value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual string GetString(int i)
        {
            return this.dataReader.GetString(i);
        }

        ///<summary>
        ///
        ///                    Return the value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The <see cref="T:System.Object" /> which will contain the field value upon return.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual object GetValue(int i)
        {
            return this.dataReader.GetValue(i);
        }

        ///<summary>
        ///
        ///                    Gets all the attribute fields in the collection for the current record.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The number of instances of <see cref="T:System.Object" /> in the array.
        ///                
        ///</returns>
        ///
        ///<param name="values">
        ///                    An array of <see cref="T:System.Object" /> to copy the attribute fields into. 
        ///                </param><filterpriority>2</filterpriority>
        public virtual int GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }

        ///<summary>
        ///
        ///                    Return whether the specified field is set to null.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if the specified field is set to null; otherwise, false.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual bool IsDBNull(int i)
        {
            return this.dataReader.IsDBNull(i);
        }

        ///<summary>
        ///
        ///                    Advances the data reader to the next result, when reading the results of batch SQL statements.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if there are more rows; otherwise, false.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool NextResult()
        {
            return this.dataReader.NextResult();
        }

        ///<summary>
        ///
        ///                    Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if there are more rows; otherwise, false.
        ///                
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool Read()
        {
            return this.dataReader.Read();
        }

        ///<summary>
        ///
        ///                    Gets the column located at the specified index.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The column located at the specified index as an <see cref="T:System.Object" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based index of the column to get. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual object this[int i]
        {
            get
            {
                return this.dataReader[i];
            }
        }

        ///<summary>
        ///
        ///                    Gets the column with the specified name.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The column with the specified name as an <see cref="T:System.Object" />.
        ///                
        ///</returns>
        ///
        ///<param name="name">
        ///                    The name of the column to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    No column with the specified name was found. 
        ///                </exception><filterpriority>2</filterpriority>
        public virtual object this[string name]
        {
            get
            {
                return this.dataReader[name];
            }
        }
    }
}