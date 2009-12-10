using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Collection
{
    /// <summary>
    /// Abstract dictionary wrapper that implements most of both the
    /// <see cref="IDictionary{TKey,TValue}"/> and the <see cref="IDictionary"/>
    /// interfaces by delegating the calls to the wrapped dictionary.
    /// </summary>
    /// <remarks>
    /// This abstract class also implements the <see cref="IXmlSerializable"/> to
    /// support XML serialization of dictionary.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractDictionaryWrapper<TKey, TValue> : AbstractDictionary<TKey, TValue>
    {
        /// <summary>
        /// Subclass must override to provide the wrapped dictionary.
        /// </summary>
        protected abstract IDictionary<TKey, TValue> Wrapped { get; }

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an element with the provided key and value to the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ///
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override void Add(TKey key, TValue value)
        {
            Wrapped.Add(key, value);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> 
        /// contains an element with the specified key.
        /// </summary>
        ///
        /// <returns>
        /// true if the <see cref="IDictionary{TKey, TValue}"/> contains an 
        /// element with the key; otherwise, false.
        /// </returns>
        ///
        /// <param name="key">
        /// The key to locate in the <see cref="IDictionary{TKey, TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override bool ContainsKey(TKey key)
        {
            return Wrapped.ContainsKey(key);
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the keys of the object that 
        /// implements <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        ///
        public override ICollection<TKey> Keys
        {
            get { return Wrapped.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ///
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  
        /// This method also returns false if key was not found in the original 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        ///
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override bool Remove(TKey key)
        {
            return Wrapped.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified 
        /// key, if the key is found; otherwise, the default value for the 
        /// type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the dictionary contains an element with the specified key; 
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override bool TryGetValue(TKey key, out TValue value)
        {
            return Wrapped.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the values in the object 
        /// that implements <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        ///
        public override ICollection<TValue> Values
        {
            get { return Wrapped.Values; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="Wrapped"/> 
        /// <see cref="Dictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key is not found.
        /// </exception>
        public override TValue this[TKey key]
        {
            get { return Wrapped[key]; }
            set { Wrapped[key] = value; }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        public override void Add(KeyValuePair<TKey, TValue> item)
        {
            Wrapped.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException">
        /// The <see cref="Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        public override void Clear()
        {
            Wrapped.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value. 
        /// </summary>
        /// 
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Wrapped.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index.
        /// </summary>
        /// 
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the 
        /// destination of the elements copied from <see cref="ICollection{T}"/>. 
        /// The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// array is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional.<br/>-or-<br/>
        /// arrayIndex is equal to or greater than the length of array. <br/>-or-<br/>
        /// The number of elements in the source <see cref="ICollection{T}"/> 
        /// is greater than the available space from arrayIndex to the end of 
        /// the destination array. <br/>-or-<br/>
        /// Type T cannot be cast automatically to the type of the destination array.
        /// </exception>
        public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Wrapped.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        public override int Count
        {
            get { return Wrapped.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="ICollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        /// 
        public override bool IsReadOnly
        {
            get { return Wrapped.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if item was successfully removed from the <see cref="ICollection{T}"/>; 
        /// otherwise, false. This method also returns false if item is not found in the 
        /// original <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Wrapped.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Wrapped.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an 
        /// <see cref="Array"/>, starting at a particular <see cref="Array"/> 
        /// index.
        /// </summary>
        ///
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination 
        /// of the elements copied from <see cref="ICollection"/>. The 
        /// <see cref="Array"/> must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        /// <exception cref="ArgumentNullException">array is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero. 
        /// </exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional.-or- index is equal to or greater than 
        /// the length of array.
        /// -or- 
        /// The number of elements in the source <see cref="ICollection"/> 
        /// is greater than the available space from index to the end of the 
        /// destination array. 
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ICollection"/> cannot be cast 
        /// automatically to the type of the destination array. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        protected override void CopyTo(Array array, int index)
        {
            IDictionary dictionary = Wrapped as IDictionary;
            if (dictionary != null) dictionary.CopyTo(array, index);
            else base.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> object for 
        /// the <see cref="IDictionary"/> object.
        /// </summary>
        /// <remarks>
        /// This implementation try to get the enumerator from the wrapped
        /// dictionary if it implements <see cref="IDictionary"/>. Otherwise,
        /// falls back to the implementation of base class.
        /// </remarks>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator"/> object for the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        protected override IDictionaryEnumerator NonGenericGetEnumerator()
        {
            IDictionary dictionary = Wrapped as IDictionary;
            return dictionary==null ? 
                base.NonGenericGetEnumerator() : 
                dictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the keys 
        /// of the <see cref="IDictionary"/> object.
        /// </summary>
        /// <remarks>
        /// This implementation try to get the keys from the wrapped
        /// dictionary if it implements <see cref="IDictionary"/>. Otherwise,
        /// falls back to the implementation of base class.
        /// </remarks>
        /// <returns>
        /// An <see cref="ICollection"/> object containing the keys of 
        /// the <see cref="IDictionary"/> object.
        /// </returns>
        protected override ICollection NonGenericKeys
        {
            get
            {
                IDictionary d = Wrapped as IDictionary;
                return d == null ? base.NonGenericKeys : d.Keys;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the values 
        /// in the <see cref="IDictionary"/> object.
        /// </summary>
        /// <remarks>
        /// This implementation try to get the values from the wrapped
        /// dictionary if it implements <see cref="IDictionary"/>. Otherwise,
        /// falls back to the implementation of base class.
        /// </remarks>
        /// <returns>
        /// An <see cref="ICollection"/> object containing the values in the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        protected override ICollection NonGenericValues
        {
            get
            {
                IDictionary d = Wrapped as IDictionary;
                return d == null ? base.NonGenericValues : d.Values;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary has a fixed size.
        /// </summary>
        /// <returns>
        /// <c>true</c> if and only if the wrapped dictionary is fixed size.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool IsFixedSize
        {
            get
            {
                IDictionary d = Wrapped as IDictionary;
                return d != null && d.IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> 
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"></see> 
        /// is synchronized (thread safe); otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        protected override bool IsSynchronized
        {
            get
            {
                IDictionary d = Wrapped as IDictionary;
                return d != null && d.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the 
        /// <see cref="T:System.Collections.ICollection"></see>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the 
        /// <see cref="T:System.Collections.ICollection"></see>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        protected override object SyncRoot
        {
            get
            {
                IDictionary d = Wrapped as IDictionary;
                return d == null ? null : d.SyncRoot;
            }
        }
    }
}
