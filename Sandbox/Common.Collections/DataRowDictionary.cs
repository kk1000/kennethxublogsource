using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Common.Collection
{
    /// <summary>
    /// A feather weight class to expouse a <see cref="DataRow"/> as 
    /// <see cref="IDictionary{String, Object}"/>.
    /// </summary>
    public class DataRowDictionary : AbstractDictionary<string, object>
    {
        DataRow _row;

        /// <summary>
        /// Construct a instance of <see cref="DataRowDictionary"/> based
        /// on <paramref name="row"/>.
        /// </summary>
        /// <param name="row"><see cref="DataRow"/> object to be wrapped</param>
        /// <exception cref="ArgumentNullException">
        /// When parameter <paramref name="row"/> is null
        /// </exception>
        public DataRowDictionary(DataRow row)
        {
            if (row == null) throw new ArgumentNullException(typeof(DataRow).FullName);
            _row = row;
        }

        /// <summary>
        /// Check if underlaying <see cref="DataRow"/> has a column
        /// named as what's specified by parameter <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Dictionary key. Also column name of 
        /// <c>DataRow</c></param>
        /// <returns><see langword="true"/> if column exist in the underlaying
        /// <c>DataRow</c>, <see langword="false"/> otherwise.</returns>
        public override bool ContainsKey(string key)
        {
            return _row.Table.Columns.Contains(key);
        }

        /// <summary>
        /// Gets a collection of strings representing the names of underlaying
        /// <see cref="DataRow"/> columns.
        /// </summary>
        public override ICollection<string> Keys
        {
            get
            {
                return new TransformingCollection<string>(
                    _row.Table.Columns,
                    delegate(object dataColumn) { return ((DataColumn)dataColumn).ColumnName; }
                );
            }
        }

        /// <summary>
        /// Determines whether the underlaying <see cref="DataRow"/> contains
        /// the column specified by the <paramref name="key"/>. If column
        /// exists, the out parameter <paramref name="value"/> is set with 
        /// the column data.
        /// </summary>
        /// <param name="key">column name of underlaying <c>DataRow</c></param>
        /// <param name="value">the column data if column exist, 
        /// <see langword="null"/> otherwise</param>
        /// <returns><see langword="true"/> if column exists in the underlaying
        /// <c>DataRow</c>, <see langword="false"/> otherwise.</returns>
        public override bool TryGetValue(string key, out object value)
        {
            bool exists = ContainsKey(key);
            value = exists ? _row[key] : null;
            return exists;
        }

        /// <summary>
        /// Gets a collection of objects representing the values of underlaying
        /// <see cref="DataRow"/>
        /// </summary>
        public override ICollection<object> Values
        {
            get
            {
                return new TransformingCollection<object>(
                    _row.Table.Columns,
                    delegate(object dataColumn) { return _row[(DataColumn)dataColumn]; }
                );
            }
        }

        /// <summary>
        /// Gets and sets the data of underlaying <see cref="DataRow"/> column
        /// specified by the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Column name of underlaying <c>DataRow</c>.</param>
        /// <returns>Data of column specified by <paramref name="key"/>.</returns>
        public override object this[string key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return _row[key];
                }
                else
                {
                    throw new KeyNotFoundException(key);
                }
            }
            set
            {
                if (ContainsKey(key))
                {
                    _row[key] = value;
                }
                else
                {
                    throw new KeyNotFoundException(key);
                }
            }
        }

        /// <summary>
        /// Determine if underlaying <see cref="DataRow"/> has the column named
        /// as <paramref name="item"/><c>.Key</c> and the data of the column
        /// equales to <paramref name="item"/><c>.Value</c>.
        /// </summary>
        /// <param name="item">Whose key and value to be compared to column name
        /// and data of the underlaying <c>DataRow</c></param>
        /// <returns><see langword="true"/> if the column is found and data equals
        /// to <paramref name="item"/> value, otherwise <see langword="false"/>
        /// </returns>
        public override bool Contains(KeyValuePair<string, object> item)
        {
            return ContainsKey(item.Key) && object.Equals(item.Value, _row[item.Key]);
        }

        /// <summary>
        /// Gets the count of columns of underlaying <see cref="DataRow"/>.
        /// </summary>
        public override int Count
        {
            get { return _row.Table.Columns.Count; }
        }

        /// <summary>
        /// Returns <see langword="false"/>.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the columns of
        /// underlaying <see cref="DataRow"/>. Each element is an
        /// <see cref="KeyValuePair{TKey,TValue}"/>, where the key is the 
        /// name of the column and value is column data.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new TransformingEnumerator<KeyValuePair<string, object>>(
                _row.Table.Columns.GetEnumerator(),
                delegate(object o)
                {
                    DataColumn c = (DataColumn)o;
                    return new KeyValuePair<string, object>(c.ColumnName, _row[c]);
                }
            );
        }
    }
}
