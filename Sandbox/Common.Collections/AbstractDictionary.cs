using System;
using System.Collections.Generic;
using System.Extension;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Collection
{
    /// <summary>
    /// Serve as based class to be inherited by the classes that needs to
    /// implement both the <see cref="IDictionary{TKey,TValue}"/> and 
    /// the <see cref="IDictionary"/> interfaces.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By inheriting from this abstract class, subclass is only required
    /// to implement the <see cref="AbstractCollection{T}.GetEnumerator()"/> and 
    /// <see cref="TryGetValue(TKey, out TValue)"/> to complete a concrete
    /// read only dictionary class.
    /// </para>
    /// <para>
    /// <see cref="AbstractDictionary{TKey,TValue}"/> throws 
    /// <see cref="NotSupportedException"/> for all access to the collection 
    /// mutating members. 
    /// </para>
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractDictionary<TKey, TValue> : AbstractCollection<KeyValuePair<TKey, TValue>>, 
        IDictionary<TKey, TValue>, IIndexable<TKey, TValue>, IDictionary, IIndexable, IXmlSerializable
    {
        /// <summary>
        /// Converter that returns the key of a given <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        protected static readonly Converter<KeyValuePair<TKey, TValue>, TKey> ToKey =
            delegate(KeyValuePair<TKey, TValue> pair) { return pair.Key; };

        /// <summary>
        /// Converter that returns the value of a given <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        protected static readonly Converter<KeyValuePair<TKey, TValue>, TValue> ToValue =
            pair => pair.Value;

        #region IDictionary<Key,Value> Members

        /// <summary>
        /// Adds an element with the provided key and value to the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// This implementation always throw <see cref="NotSupportedException"/>.
        /// </summary>
        ///
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public virtual void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds the key value <paramref name="pair"/> to the dictionary.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey,TValue}"/> to add to the 
        /// <see cref="IDictionary{TKey,TValue}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IDictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        public override void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> 
        /// contains an element with the specified key.
        /// This implementation uses <see cref="TryGetValue(TKey, out TValue)"/>.
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
        public virtual bool ContainsKey(TKey key)
        {
            TValue v;
            return TryGetValue(key, out v);
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// This implementation returns a transformed collection based on current
        /// dictionary instance.
        /// See <see cref="TransformingCollection{TFrom,TTo}"/>.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the keys of the object that 
        /// implements <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        ///
        public virtual ICollection<TKey> Keys
        {
            get {
                return new TransformingCollection<KeyValuePair<TKey, TValue>, TKey>(this, ToKey);
            }
        }

        /// <summary>
        /// Removes the specific <see cref="KeyValuePair{TKey,TValue}"/> from dictionary.
        /// </summary>
        /// <returns>
        /// true if the key value pair is found and removed from the dictionary; 
        /// otherwise, false.
        /// </returns>
        /// <param name="item">
        /// The key value pair to remove from the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="IDictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue result;
            var exists = TryGetValue(item.Key, out result);
            if (exists && Equals(item.Value, result))
            {
                return Remove(item.Key);
            }
            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// This implementation always throw <see cref="NotSupportedException"/>
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
        /// The <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public virtual bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// This is an abstract method that each concrete subclass must 
        /// implement.
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
        public abstract bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// This implementation returns a transformed collection based on
        /// current dictionary instance. 
        /// See <see cref="TransformingCollection{TFrom,TTo}"/>.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the values in the object 
        /// that implements <see cref="IDictionary{TKey, TValue}"/>.
        /// </returns>
        ///
        public virtual ICollection<TValue> Values
        {
            get
            {
                return new TransformingCollection<KeyValuePair<TKey, TValue>, TValue>(this, ToValue);
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <remarks>
        /// The setter is not supported. The getter calls 
        /// <see cref="TryGetValue(TKey, out TValue)"/> to retrieve the value.
        /// </remarks>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="Dictionary{TKey,TValue}"/> 
        /// is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key is not found.
        /// </exception>
        public virtual TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException(key.ToString());
                }
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region IDictionary Members

        /// <summary>
        /// Adds an element with the provided key and value to the 
        /// <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to use as the value of the element to add. 
        /// </param>
        /// <param name="key">
        /// The <see cref="T:System.Object"/> to use as the key of the element to add. 
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the 
        /// <see cref="IDictionary"/> object. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// key is null. 
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IDictionary"/> is read-only.
        /// -or- 
        /// The <see cref="IDictionary"/> has a fixed size. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary"/> 
        /// object contains an element with the specified key.
        /// </summary>
        ///
        /// <returns>
        /// true if the <see cref="IDictionary"/> 
        /// contains an element with the key; otherwise, false.
        /// </returns>
        ///
        /// <param name="key">
        /// The key to locate in the <see cref="IDictionary"/> object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// key is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        bool IDictionary.Contains(object key)
        {
            return (key is TKey) && ContainsKey((TKey)key);
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> object for 
        /// the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IDictionaryEnumerator"/> object for the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return NonGenericGetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary has a fixed size.
        /// </summary>
        /// <remarks>
        /// Subclass that is fixed in size should override this method.
        /// </remarks>
        /// <returns>This implementation always return false.</returns>
        /// <filterpriority>2</filterpriority>
        public virtual bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the keys 
        /// of the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection"/> object containing the keys of 
        /// the <see cref="IDictionary"/> object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        ICollection IDictionary.Keys
        {
            get { return NonGenericKeys; }
        }


        /// <summary>
        /// Removes the element with the specified key from the 
        /// <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <param name="key">The key of the element to remove. </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IDictionary"/> object is read-only.
        /// -or- 
        /// The <see cref="IDictionary"/> has a fixed size. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// key is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        void IDictionary.Remove(object key)
        {
            if (key==null) throw new ArgumentNullException("key");
            if (key is TKey) Remove((TKey)key);
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the values 
        /// in the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection"/> object containing the values in the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        ICollection IDictionary.Values
        {
            get { return NonGenericValues; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        ///
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        ///
        /// <param name="key">
        /// The key of the element to get or set. 
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="IDictionary"/> object 
        /// is read-only.-or- The property is set, key does not exist in 
        /// the collection, and the <see cref="IDictionary"/> has a fixed 
        /// size. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// key is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        object IDictionary.this[object key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");
                return NonGenericIndexerGet(key);
            }
            set
            {
                if (key == null) throw new ArgumentNullException("key");
                NonGenericIndexerSet(key, value);
            }
        }

        /// <summary>
        /// Gets or sets the data with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The index of the data to get or set</param>
        /// <returns>
        /// The data with the specified <paramref name="key"/>
        /// </returns>
        object IIndexable.this[object key]
        {
            get { return NonGenericIndexerGet(key); }
            set { NonGenericIndexerSet(key, value); }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> 
        /// to the class instead. 
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes 
        /// the XML representation of the object that is produced by the 
        /// <see cref="IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> 
        /// method and consumed by the 
        /// <see cref="IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        ///
        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        ///
        /// <param name="reader">
        /// The <see cref="T:System.Xml.XmlReader"/> stream from which the 
        /// object is deserialized. 
        /// </param>
        public virtual void ReadXml(XmlReader reader)
        {
            Collections.ReadXmlToDictionary(reader, this);
        }

        /// <summary>
        ///Converts an object into its XML representation.
        /// </summary>
        ///
        /// <param name="writer">
        /// The <see cref="T:System.Xml.XmlWriter"/> stream to which the 
        /// object is serialized. 
        /// </param>
        public virtual void WriteXml(XmlWriter writer)
        {
            Collections.WriteDictionaryToXml(writer, this);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="string"/> that represents the dictionary.
        /// </summary>
        /// <remarks>
        /// This implmentation list out all the elements separated by comma.
        /// </remarks>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name).Append("(");
            return Collections.ToString((IDictionary<TKey, TValue>)this, sb).Append(")").ToString();
        }


        /// <summary>
        /// This is convenient method to be used by the subclass that can
        /// provide the key collection easily but difficult to implement the
        /// <see cref="AbstractCollection{T}.GetEnumerator()"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/>
        /// Instance that can be used by the implementation of method
        /// <see cref="AbstractCollection{T}.GetEnumerator()"/>.
        /// </returns>
        protected IEnumerator<KeyValuePair<TKey, TValue>> GetEnumeratorFromKeys()
        {
            return new TransformingEnumerator<TKey, KeyValuePair<TKey,TValue>>(
                Keys.GetEnumerator(),
                delegate(TKey key) { return new KeyValuePair<TKey, TValue>(key, this[key]); }
            );
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the keys 
        /// of the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection"/> object containing the keys of 
        /// the <see cref="IDictionary"/> object.
        /// </returns>
        protected virtual ICollection NonGenericKeys
        {
            get
            {
                ICollection<TKey> keys = Keys;

                return (keys is ICollection) ? (ICollection)keys :
                    new CollectionWrapper<TKey>(keys);
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the values 
        /// in the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ICollection"/> object containing the values in the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        protected virtual ICollection NonGenericValues
        {
            get
            {
                ICollection<TValue> values = Values;

                return (values is ICollection) ? (ICollection)values :
                    new CollectionWrapper<TValue>(values);
            }
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> object for 
        /// the <see cref="IDictionary"/> object.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IDictionaryEnumerator"/> object for the 
        /// <see cref="IDictionary"/> object.
        /// </returns>
        protected virtual IDictionaryEnumerator NonGenericGetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(GetEnumerator());
        }

        /// <summary>
        /// Gets the element with the specified <paramref name="key"/>.
        /// </summary>
        ///
        /// <returns>
        /// The element with the specified <paramref name="key"/>.
        /// </returns>
        ///
        /// <param name="key">
        /// The key of the element to get. 
        /// </param>
        protected virtual object NonGenericIndexerGet(object key)
        {
            TValue value;
            return (key is TKey) && TryGetValue((TKey)key, out value) ? value : (object)null;
        }

        /// <summary>
        /// Sets the element with the specified <paramref name="key"/>.
        /// </summary>
        ///
        /// <param name="key">
        /// The key of the element to set. 
        /// </param>
        /// <param name="value">
        /// The value to be set.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="this[TKey]"/> throws the 
        /// same.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// When <paramref name="key"/> is not assignable to 
        /// <typeparamref name="TKey"/> or <paramref name="value"/> is not
        /// assignable to <typeparamref name="TValue"/>.
        /// </exception>
        protected virtual void NonGenericIndexerSet(object key, object value)
        {
            this[(TKey)key] = (TValue)value;
        }
    }
}
