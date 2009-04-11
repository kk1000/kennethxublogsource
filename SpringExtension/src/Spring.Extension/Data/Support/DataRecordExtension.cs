using System;
using System.Data;

namespace Spring.Data.Support
{
    /// <summary>
    /// Extension methods for <see cref="IDataRecord"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This provides the convenient operations described in below sections.
    /// </para>
    /// <para>
    /// SafeGetXyz methods that return default value when <see cref="DBNull"/>
    /// is encountered.
    /// </para>
    /// <para>
    /// GetNullableXyz methods that return a <see cref="Nullable{T}"/> type
    /// of the corresponding value types to handle <see cref="DBNull"/>.
    /// </para>
    /// <para>
    /// Provide getter methods that indexed by field name instead of position.
    /// Those methods uses <see cref="IDataRecord.GetOrdinal"/> to obtain
    /// the position of field name. Due to this, those getters are generally
    /// slower then their position based couterpart, especially when the
    /// implementation of <see cref="IDataRecord.GetOrdinal"/> method is
    /// inefficient. <see cref="DataRecordByNameReader"/> for faster field 
    /// name based operations.
    /// </para>
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public static class DataRecordExtension
    {
        #region GetBoolean

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public static bool GetBoolean(this IDataRecord record, int index, string trueValue)
        {
            return StringToBoolean(record.GetString(index), trueValue, null);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool GetBoolean(this IDataRecord record, int index, string trueValue, string falseValue)
        {
            return StringToBoolean(record.GetString(index), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public static bool GetBoolean(this IDataRecord record, int index, char trueValue)
        {
            return CharToBoolean(record.GetChar(index), trueValue, default(char?));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool GetBoolean(this IDataRecord record, int index, char trueValue, char? falseValue)
        {
            return CharToBoolean(record.GetChar(index), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static bool SafeGetBoolean(this IDataRecord record, int index, bool defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetBoolean(index);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the specified field, or 
        /// <c>default(bool)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static bool SafeGetBoolean(this IDataRecord record, int index)
        {
            return SafeGetBoolean(record, index, default(bool));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public static bool SafeGetBoolean(this IDataRecord record, int index, string trueValue)
        {
            return SafeGetBoolean(record, index, trueValue, null);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool SafeGetBoolean(this IDataRecord record, int index, string trueValue, string falseValue)
        {
            string value = SafeGetString(record, index);
            return value == null ? false : StringToBoolean(value, trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>, otherwise <c>false</c>.
        /// </returns>
        public static bool SafeGetBoolean(this IDataRecord record, int index, char trueValue)
        {
            return SafeGetBoolean(record, index, trueValue, default(char?));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool SafeGetBoolean(this IDataRecord record, int index, char trueValue, char? falseValue)
        {
            char? value = GetNullableChar(record, index);
            return value.HasValue ? CharToBoolean(value.Value, trueValue, falseValue) : false;
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="bool"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="bool"/> value of the 
        /// specified field, or <c>default(bool?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(bool?) : record.GetBoolean(index);
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="bool"/> value of 
        /// the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>null</c> if the value of the
        /// field is <c>null</c>; otherwise <c>false</c>.
        /// </returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int index, string trueValue)
        {
            return GetNullableBoolean(record, index, trueValue, null);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of string type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool? GetNullableBoolean(this IDataRecord record, int index, string trueValue, string falseValue)
        {
            return StringToNullableBoolean(SafeGetString(record, index), trueValue, falseValue);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="trueValue">
        /// The value of the field to be considered as <c>true</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value of the specified field equals to the 
        /// <paramref name="trueValue"/>; <c>null</c> if the field is null;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int index, char trueValue)
        {
            return GetNullableBoolean(record, index, trueValue, default(char?));
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of the specified field of char type.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
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
        public static bool? GetNullableBoolean(this IDataRecord record, int index, char trueValue, char? falseValue)
        {
            return CharToNullableBoolean(GetNullableChar(record, index), trueValue, falseValue);
        }

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
        public static bool GetBoolean(this IDataRecord record, string name)
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
        public static bool GetBoolean(this IDataRecord record, string name, string trueValue)
        {
            return GetBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool GetBoolean(this IDataRecord record, string name, string trueValue, string falseValue)
        {
            return GetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
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
        public static bool GetBoolean(this IDataRecord record, string name, char trueValue)
        {
            return GetBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool GetBoolean(this IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return GetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
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
        public static bool SafeGetBoolean(this IDataRecord record, string name, bool defaultValue)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name), defaultValue);
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
        public static bool SafeGetBoolean(this IDataRecord record, string name)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name));
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
        public static bool SafeGetBoolean(this IDataRecord record, string name, string trueValue)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool SafeGetBoolean(this IDataRecord record, string name, string trueValue, string falseValue)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
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
        public static bool SafeGetBoolean(this IDataRecord record, string name, char trueValue)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool SafeGetBoolean(this IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return SafeGetBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
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
        public static bool? GetNullableBoolean(this IDataRecord record, string name)
        {
            return GetNullableBoolean(record, GetOrdinal(record, name));
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
        public static bool? GetNullableBoolean(this IDataRecord record, string name, string trueValue)
        {
            return GetNullableBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool? GetNullableBoolean(this IDataRecord record, string name, string trueValue, string falseValue)
        {
            return GetNullableBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
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
        public static bool? GetNullableBoolean(this IDataRecord record, string name, char trueValue)
        {
            return GetNullableBoolean(record, GetOrdinal(record, name), trueValue);
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
        public static bool? GetNullableBoolean(this IDataRecord record, string name, char trueValue, char? falseValue)
        {
            return GetNullableBoolean(record, GetOrdinal(record, name), trueValue, falseValue);
        }

        #endregion

        #region GetByte

        /// <summary>
        /// Gets the <see cref="byte"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static byte SafeGetByte(this IDataRecord record, int index, byte defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetByte(index);
        }

        /// <summary>
        /// Gets the <see cref="byte"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> value of the specified field, or 
        /// <c>default(byte)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static byte SafeGetByte(this IDataRecord record, int index)
        {
            return SafeGetByte(record, index, default(byte));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="byte"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="byte"/> value of the 
        /// specified field, or <c>default(byte?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static byte? GetNullableByte(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(byte?) : record.GetByte(index);
        }

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
        public static byte GetByte(this IDataRecord record, string name)
        {
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
        public static byte SafeGetByte(this IDataRecord record, string name, byte defaultValue)
        {
            return SafeGetByte(record, GetOrdinal(record, name), defaultValue);
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
        public static byte SafeGetByte(this IDataRecord record, string name)
        {
            return SafeGetByte(record, GetOrdinal(record, name));
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
        public static byte? GetNullableByte(this IDataRecord record, string name)
        {
            return GetNullableByte(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetChar

        /// <summary>
        /// Gets the <see cref="char"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="char"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static char SafeGetChar(this IDataRecord record, int index, char defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetChar(index);
        }

        /// <summary>
        /// Gets the <see cref="char"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="char"/> value of the specified field, or 
        /// <c>default(char)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static char SafeGetChar(this IDataRecord record, int index)
        {
            return SafeGetChar(record, index, default(char));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="char"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="char"/> value of the 
        /// specified field, or <c>default(char?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static char? GetNullableChar(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(char?) : record.GetChar(index);
        }

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
        public static char GetChar(this IDataRecord record, string name)
        {
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
        public static char SafeGetChar(this IDataRecord record, string name, char defaultValue)
        {
            return SafeGetChar(record, GetOrdinal(record, name), defaultValue);
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
        public static char SafeGetChar(this IDataRecord record, string name)
        {
            return SafeGetChar(record, GetOrdinal(record, name));
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
        public static char? GetNullableChar(this IDataRecord record, string name)
        {
            return GetNullableChar(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDateTime

        /// <summary>
        /// Gets the <see cref="DateTime"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static DateTime SafeGetDateTime(this IDataRecord record, int index, DateTime defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetDateTime(index);
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/> value of the specified field, or 
        /// <c>default(DateTime)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static DateTime SafeGetDateTime(this IDataRecord record, int index)
        {
            return SafeGetDateTime(record, index, default(DateTime));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="DateTime"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="DateTime"/> value of the 
        /// specified field, or <c>default(DateTime?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(DateTime?) : record.GetDateTime(index);
        }

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
        public static DateTime GetDateTime(this IDataRecord record, string name)
        {
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
        public static DateTime SafeGetDateTime(this IDataRecord record, string name, DateTime defaultValue)
        {
            return SafeGetDateTime(record, GetOrdinal(record, name), defaultValue);
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
        public static DateTime SafeGetDateTime(this IDataRecord record, string name)
        {
            return SafeGetDateTime(record, GetOrdinal(record, name));
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
        public static DateTime? GetNullableDateTime(this IDataRecord record, string name)
        {
            return GetNullableDateTime(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDecimal

        /// <summary>
        /// Gets the <see cref="decimal"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static decimal SafeGetDecimal(this IDataRecord record, int index, decimal defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetDecimal(index);
        }

        /// <summary>
        /// Gets the <see cref="decimal"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> value of the specified field, or 
        /// <c>default(decimal)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static decimal SafeGetDecimal(this IDataRecord record, int index)
        {
            return SafeGetDecimal(record, index, default(decimal));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="decimal"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="decimal"/> value of the 
        /// specified field, or <c>default(decimal?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(decimal?) : record.GetDecimal(index);
        }

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
        public static decimal GetDecimal(this IDataRecord record, string name)
        {
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
        public static decimal SafeGetDecimal(this IDataRecord record, string name, decimal defaultValue)
        {
            return SafeGetDecimal(record, GetOrdinal(record, name), defaultValue);
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
        public static decimal SafeGetDecimal(this IDataRecord record, string name)
        {
            return SafeGetDecimal(record, GetOrdinal(record, name));
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
        public static decimal? GetNullableDecimal(this IDataRecord record, string name)
        {
            return GetNullableDecimal(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetDouble

        /// <summary>
        /// Gets the <see cref="double"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static double SafeGetDouble(this IDataRecord record, int index, double defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetDouble(index);
        }

        /// <summary>
        /// Gets the <see cref="double"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> value of the specified field, or 
        /// <c>default(double)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static double SafeGetDouble(this IDataRecord record, int index)
        {
            return SafeGetDouble(record, index, default(double));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="double"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="double"/> value of the 
        /// specified field, or <c>default(double?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static double? GetNullableDouble(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(double?) : record.GetDouble(index);
        }

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
        public static double GetDouble(this IDataRecord record, string name)
        {
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
        public static double SafeGetDouble(this IDataRecord record, string name, double defaultValue)
        {
            return SafeGetDouble(record, GetOrdinal(record, name), defaultValue);
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
        public static double SafeGetDouble(this IDataRecord record, string name)
        {
            return SafeGetDouble(record, GetOrdinal(record, name));
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
        public static double? GetNullableDouble(this IDataRecord record, string name)
        {
            return GetNullableDouble(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetFloat

        /// <summary>
        /// Gets the <see cref="float"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="float"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static float SafeGetFloat(this IDataRecord record, int index, float defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetFloat(index);
        }

        /// <summary>
        /// Gets the <see cref="float"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="float"/> value of the specified field, or 
        /// <c>default(float)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static float SafeGetFloat(this IDataRecord record, int index)
        {
            return SafeGetFloat(record, index, default(float));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="float"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="float"/> value of the 
        /// specified field, or <c>default(float?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static float? GetNullableFloat(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(float?) : record.GetFloat(index);
        }

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
        public static float GetFloat(this IDataRecord record, string name)
        {
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
        public static float SafeGetFloat(this IDataRecord record, string name, float defaultValue)
        {
            return SafeGetFloat(record, GetOrdinal(record, name), defaultValue);
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
        public static float SafeGetFloat(this IDataRecord record, string name)
        {
            return SafeGetFloat(record, GetOrdinal(record, name));
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
        public static float? GetNullableFloat(this IDataRecord record, string name)
        {
            return GetNullableFloat(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt16

        /// <summary>
        /// Gets the <see cref="short"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="short"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static short SafeGetInt16(this IDataRecord record, int index, short defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetInt16(index);
        }

        /// <summary>
        /// Gets the <see cref="short"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="short"/> value of the specified field, or 
        /// <c>default(short)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static short SafeGetInt16(this IDataRecord record, int index)
        {
            return SafeGetInt16(record, index, default(short));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="short"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="short"/> value of the 
        /// specified field, or <c>default(short?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static short? GetNullableInt16(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(short?) : record.GetInt16(index);
        }

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
        public static short GetInt16(this IDataRecord record, string name)
        {
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
        public static short SafeGetInt16(this IDataRecord record, string name, short defaultValue)
        {
            return SafeGetInt16(record, GetOrdinal(record, name), defaultValue);
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
        public static short SafeGetInt16(this IDataRecord record, string name)
        {
            return SafeGetInt16(record, GetOrdinal(record, name));
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
        public static short? GetNullableInt16(this IDataRecord record, string name)
        {
            return GetNullableInt16(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt32

        /// <summary>
        /// Gets the <see cref="int"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static int SafeGetInt32(this IDataRecord record, int index, int defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetInt32(index);
        }

        /// <summary>
        /// Gets the <see cref="int"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> value of the specified field, or 
        /// <c>default(int)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static int SafeGetInt32(this IDataRecord record, int index)
        {
            return SafeGetInt32(record, index, default(int));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="int"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="int"/> value of the 
        /// specified field, or <c>default(int?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static int? GetNullableInt32(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(int?) : record.GetInt32(index);
        }

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
        public static int GetInt32(this IDataRecord record, string name)
        {
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
        public static int SafeGetInt32(this IDataRecord record, string name, int defaultValue)
        {
            return SafeGetInt32(record, GetOrdinal(record, name), defaultValue);
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
        public static int SafeGetInt32(this IDataRecord record, string name)
        {
            return SafeGetInt32(record, GetOrdinal(record, name));
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
        public static int? GetNullableInt32(this IDataRecord record, string name)
        {
            return GetNullableInt32(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetInt64

        /// <summary>
        /// Gets the <see cref="long"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="long"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static long SafeGetInt64(this IDataRecord record, int index, long defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetInt64(index);
        }

        /// <summary>
        /// Gets the <see cref="long"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="long"/> value of the specified field, or 
        /// <c>default(long)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static long SafeGetInt64(this IDataRecord record, int index)
        {
            return SafeGetInt64(record, index, default(long));
        }

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> of <see cref="long"/> value of 
        /// the specified field.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> of <see cref="long"/> value of the 
        /// specified field, or <c>default(long?)</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static long? GetNullableInt64(this IDataRecord record, int index)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? default(long?) : record.GetInt64(index);
        }

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
        public static long GetInt64(this IDataRecord record, string name)
        {
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
        public static long SafeGetInt64(this IDataRecord record, string name, long defaultValue)
        {
            return SafeGetInt64(record, GetOrdinal(record, name), defaultValue);
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
        public static long SafeGetInt64(this IDataRecord record, string name)
        {
            return SafeGetInt64(record, GetOrdinal(record, name));
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
        public static long? GetNullableInt64(this IDataRecord record, string name)
        {
            return GetNullableInt64(record, GetOrdinal(record, name));
        }

        #endregion

        #region GetString

        /// <summary>
        /// Gets the <see cref="string"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <param name="defaultValue">
        /// Default value to return when <see cref="DBNull"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the specified field, or 
        /// <paramref name="defaultValue"/> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static string SafeGetString(this IDataRecord record, int index, string defaultValue)
        {
            AssertRecordNotNull(record);
            return record.IsDBNull(index) ? defaultValue : record.GetString(index);
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of the specified field safely.
        /// </summary>
        /// <param name="record">
        /// An <see cref="IDataRecord"/> to get value from.
        /// </param>
        /// <param name="index">
        /// The index of the field to find.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the specified field, or 
        /// <c>null</c> if the value of the field is 
        /// <see cref="DBNull"/>.
        /// </returns>
        public static string SafeGetString(this IDataRecord record, int index)
        {
            return SafeGetString(record, index, null);
        }

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
        public static string GetString(this IDataRecord record, string name)
        {
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
        public static string SafeGetString(this IDataRecord record, string name, string defaultValue)
        {
            return SafeGetString(record, GetOrdinal(record, name), defaultValue);
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
        public static string SafeGetString(this IDataRecord record, string name)
        {
            return SafeGetString(record, GetOrdinal(record, name));
        }

        #endregion

        #region Boolean Converters
        public static bool? StringToNullableBoolean(string value, string trueValue, string falseValue)
        {
            AssertTrueValueNotNull(trueValue);
            if (value == null) return null;

            if (value.Equals(trueValue)) return true;
            if (falseValue == null && value.Equals(falseValue)) return false;
            return null;
        }

        public static bool? CharToNullableBoolean(char? value, char trueValue, char? falseValue)
        {
            if (!value.HasValue) return null;
            if (value == trueValue) return true;
            if (!falseValue.HasValue || falseValue.Value == value) return false;
            return null;
        }

        public static bool StringToBoolean(string value, string trueValue, string falseValue)
        {
            AssertValueNotNull(value);
            AssertTrueValueNotNull(trueValue);

            if (value.Equals(trueValue)) return true;
            if (falseValue == null && value.Equals(falseValue)) return false;
            throw new ArgumentException(String.Format(
                                            "Unable to convert string '{0}' to boolean. Expecting {1} or {2}.", value, trueValue, falseValue));
        }

        public static bool CharToBoolean(char value, char trueValue, char? falseValue)
        {
            if (value == trueValue) return true;
            if (!falseValue.HasValue || falseValue.Value == value) return false;
            throw new ArgumentException(String.Format(
                                            "Unable to convert char '{0}' to boolean. Expecting {1} or {2}.", value, trueValue, falseValue));
        }
        #endregion

        #region Private Methods

        private static int GetOrdinal(IDataRecord record, string name)
        {
            AssertRecordNotNull(record);
            if (name == null) throw new ArgumentNullException("name");
            return record.GetOrdinal(name);
        }

        private static void AssertRecordNotNull(IDataRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");
        }

        private static void AssertTrueValueNotNull(string trueValue)
        {
            if (trueValue == null) throw new ArgumentNullException("trueValue");
        }

        private static void AssertValueNotNull(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
        }

        #endregion
    }
}