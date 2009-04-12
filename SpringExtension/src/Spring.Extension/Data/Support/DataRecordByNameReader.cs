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
    /// Class to enhance the performance of getting data from 
    /// <see cref="IDataRecord"/> by field name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provide getter methods that indexed by field name instead of position.
    /// Those methods are instance methods so that field name to position
    /// mapping is cached for best performance.
    /// </para>
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public class DataRecordByNameReader
    {
        private volatile IDictionary<string, int> _ordinalMap;

        #region GetBoolean

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the specified field, or 
        /// <c>default(bool)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public bool GetBoolean(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetBoolean(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public bool GetBoolean(IDataRecord record, string name, string trueValue)
        {
            return DataRecordExtension.GetBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if the value equals to
        /// <paramref name="falseValue"/> or <c>falseValue</c> is equals to
        /// <c>null</c>; otherwise an <see cref="ArgumentException"/>
        /// is thrown.
        /// </returns>
        public bool GetBoolean(IDataRecord record, string name, string trueValue, string falseValue)
        {
            return DataRecordExtension.GetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public bool GetBoolean(IDataRecord record, string name, char trueValue)
        {
            return DataRecordExtension.GetBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if the field value
        /// equals to <paramref name="falseValue"/> or <c>falseValue</c>
        /// is <c>null</c>; otherwise an <see cref="ArgumentException"/>
        /// is thrown.
        /// </returns>
        public bool GetBoolean(IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return DataRecordExtension.GetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name, bool defaultValue)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the specified field, or 
        /// <c>default(bool)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name, string trueValue)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if value is null, the
        /// <paramref name="falseValue"/> is null, or the value equals to
        /// <paramref name="falseValue"/>; otherwise an <see cref="ArgumentException"/>
        /// is thrown.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name, string trueValue, string falseValue)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name, char trueValue)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if value is null,
        /// <paramref name="falseValue"/> is null, or value equals to
        /// <paramref name="falseValue"/>; otherwise an <see cref="ArgumentException"/>
        /// is thrown.
        /// </returns>
        public bool SafeGetBoolean(IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return DataRecordExtension.SafeGetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="bool"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="bool"/> value of the 
        /// specified field, or <c>default(bool?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public bool? GetNullableBoolean(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableBoolean(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="bool"/> value of 
        /// the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>null</c> if the value of the
        /// field is <c>null</c>; otherwise <c>false</c>.
        /// </returns>
        public bool? GetNullableBoolean(IDataRecord record, string name, string trueValue)
        {
            return DataRecordExtension.GetNullableBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if the field value
        /// equals to <paramref name="falseValue"/> or <c>falseValue</c>
        /// is <c>null</c>; otherwise, return <c>null</c>.
        /// </returns>
        public bool? GetNullableBoolean(IDataRecord record, string name, string trueValue, string falseValue)
        {
            return DataRecordExtension.GetNullableBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>null</c> if the field is null;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool? GetNullableBoolean(IDataRecord record, string name, char trueValue)
        {
            return DataRecordExtension.GetNullableBoolean(record, GetOrdinal(record, name), trueValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <param name="falseValue">
        /// The value of the field to be considered as <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>false</c> if the field value
        /// equals to <paramref name="falseValue"/> or <c>falseValue</c>
        /// is <c>null</c>; otherwise, return <c>null</c>.
        /// </returns>
        public bool? GetNullableBoolean(IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return DataRecordExtension.GetNullableBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        #endregion

        #region GetByte

        /// <summary>
        /// Gets the <see cref="byte"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> value of the specified field, or 
        /// <c>default(byte)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public byte GetByte(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetByte(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="byte"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public byte SafeGetByte(IDataRecord record, string name, byte defaultValue)
        {
            return DataRecordExtension.SafeGetByte(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="byte"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> value of the specified field, or 
        /// <c>default(byte)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public byte SafeGetByte(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetByte(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="byte"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="byte"/> value of the 
        /// specified field, or <c>default(byte?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public byte? GetNullableByte(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableByte(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetChar

        /// <summary>
        /// Gets the <see cref="char"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="char"/> value of the specified field, or 
        /// <c>default(char)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public char GetChar(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetChar(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="char"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="char"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public char SafeGetChar(IDataRecord record, string name, char defaultValue)
        {
            return DataRecordExtension.SafeGetChar(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="char"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="char"/> value of the specified field, or 
        /// <c>default(char)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public char SafeGetChar(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetChar(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="char"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="char"/> value of the 
        /// specified field, or <c>default(char?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public char? GetNullableChar(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableChar(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDateTime

        /// <summary>
        /// Gets the <see cref="DateTime"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/> value of the specified field, or 
        /// <c>default(DateTime)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public DateTime GetDateTime(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetDateTime(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public DateTime SafeGetDateTime(IDataRecord record, string name, DateTime defaultValue)
        {
            return DataRecordExtension.SafeGetDateTime(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/> value of the specified field, or 
        /// <c>default(DateTime)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public DateTime SafeGetDateTime(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetDateTime(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="DateTime"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="DateTime"/> value of the 
        /// specified field, or <c>default(DateTime?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public DateTime? GetNullableDateTime(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableDateTime(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDecimal

        /// <summary>
        /// Gets the <see cref="decimal"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> value of the specified field, or 
        /// <c>default(decimal)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public decimal GetDecimal(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetDecimal(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="decimal"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public decimal SafeGetDecimal(IDataRecord record, string name, decimal defaultValue)
        {
            return DataRecordExtension.SafeGetDecimal(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="decimal"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> value of the specified field, or 
        /// <c>default(decimal)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public decimal SafeGetDecimal(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetDecimal(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="decimal"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="decimal"/> value of the 
        /// specified field, or <c>default(decimal?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public decimal? GetNullableDecimal(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableDecimal(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDouble

        /// <summary>
        /// Gets the <see cref="double"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> value of the specified field, or 
        /// <c>default(double)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public double GetDouble(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetDouble(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="double"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public double SafeGetDouble(IDataRecord record, string name, double defaultValue)
        {
            return DataRecordExtension.SafeGetDouble(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="double"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> value of the specified field, or 
        /// <c>default(double)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public double SafeGetDouble(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetDouble(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="double"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="double"/> value of the 
        /// specified field, or <c>default(double?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public double? GetNullableDouble(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableDouble(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetFloat

        /// <summary>
        /// Gets the <see cref="float"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="float"/> value of the specified field, or 
        /// <c>default(float)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public float GetFloat(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetFloat(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="float"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="float"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public float SafeGetFloat(IDataRecord record, string name, float defaultValue)
        {
            return DataRecordExtension.SafeGetFloat(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="float"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="float"/> value of the specified field, or 
        /// <c>default(float)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public float SafeGetFloat(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetFloat(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="float"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="float"/> value of the 
        /// specified field, or <c>default(float?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public float? GetNullableFloat(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableFloat(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt16

        /// <summary>
        /// Gets the <see cref="short"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="short"/> value of the specified field, or 
        /// <c>default(short)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public short GetInt16(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetInt16(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="short"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="short"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public short SafeGetInt16(IDataRecord record, string name, short defaultValue)
        {
            return DataRecordExtension.SafeGetInt16(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="short"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="short"/> value of the specified field, or 
        /// <c>default(short)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public short SafeGetInt16(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetInt16(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="short"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="short"/> value of the 
        /// specified field, or <c>default(short?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public short? GetNullableInt16(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableInt16(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt32

        /// <summary>
        /// Gets the <see cref="int"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> value of the specified field, or 
        /// <c>default(int)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public int GetInt32(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetInt32(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="int"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public int SafeGetInt32(IDataRecord record, string name, int defaultValue)
        {
            return DataRecordExtension.SafeGetInt32(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="int"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> value of the specified field, or 
        /// <c>default(int)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public int SafeGetInt32(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetInt32(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="int"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="int"/> value of the 
        /// specified field, or <c>default(int?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public int? GetNullableInt32(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableInt32(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt64

        /// <summary>
        /// Gets the <see cref="long"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="long"/> value of the specified field, or 
        /// <c>default(long)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public long GetInt64(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetInt64(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="long"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="long"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public long SafeGetInt64(IDataRecord record, string name, long defaultValue)
        {
            return DataRecordExtension.SafeGetInt64(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="long"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="long"/> value of the specified field, or 
        /// <c>default(long)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public long SafeGetInt64(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetInt64(record, GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="long"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="long"/> value of the 
        /// specified field, or <c>default(long?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public long? GetNullableInt64(IDataRecord record, string name)
        {
            return DataRecordExtension.GetNullableInt64(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetString

        /// <summary>
        /// Gets the <see cref="string"/> value of the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the specified field, or 
        /// <c>default(string)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public string GetString(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            return record.GetString(GetOrdinal(record, name));
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public string SafeGetString(IDataRecord record, string name, string defaultValue)
        {
            return DataRecordExtension.SafeGetString(record, GetOrdinal(record, name), defaultValue);
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="name">
        /// The name of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the specified field, or 
        /// <c>default(string)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public string SafeGetString(IDataRecord record, string name)
        {
            return DataRecordExtension.SafeGetString(record, GetOrdinal(record, name));
        }

        #endregion

        #region Private Instance Methods

        private int GetOrdinal(IDataRecord record, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (_ordinalMap == null)
            {
                lock (this)
                {
                    _ordinalMap = BuildOrdinalMap(record);
                }
            }
            int index;
            if (_ordinalMap.TryGetValue(name, out index)) return index;
            if (_ordinalMap.TryGetValue(name.ToUpper(), out index)) return index;
            return record.GetOrdinal(name);
        }

        #endregion

        #region Private Static Methods

        private static IDictionary<string, int> BuildOrdinalMap(IDataRecord record)
        {
            int count = record.FieldCount;
            IDictionary<string, int> map = new Dictionary<string, int>(count);
            for (int i = 0; i < count; i++)
            {
                map[record.GetName(i)] = i;
                map[record.GetName(i).ToLower()] = i;
                map[record.GetName(i).ToUpper()] = i;
            }
            return map;
        }

        private static void AssertRecordNotNull(IDataRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");
        }

        #endregion

    }
}