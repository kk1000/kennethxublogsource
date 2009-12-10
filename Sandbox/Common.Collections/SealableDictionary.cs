using System;
using System.Collections.Generic;
using System.Collections;
using System.Extension;
using System.Xml.Serialization;
using System.Text;

namespace Common.Collection
{
    /// <summary>
    /// A dictionary class that wraps another dictionary and can be sealed.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <author>Kenneth Xu</author>
    [XmlRoot("Dictionary")]
    public class SealableDictionary<TKey, TValue> : 
        DictionaryWrapper<TKey, TValue>, ISealableDictionary<TKey, TValue>, ISealableDictionary
    {

        /// <summary>
        /// A volatile flag to indicate that the instance is sealed.
        /// </summary>
        protected volatile bool m_Sealed = false;

        /// <summary>
        /// Construct a new instance wraps the given <paramref name="dictionary"/>.
        /// </summary>
        /// <param name="dictionary">
        /// The original dictionary to be wrapped as sealable.
        /// </param>
        public SealableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Construct a new instance wraps a new instance of
        /// <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        public SealableDictionary()
            : base(new Dictionary<TKey, TValue>())
        {
        }


        #region ISealable Members

        /// <summary>
        /// implements <see cref="ISealable.Seal"/>
        /// </summary>
        public virtual void Seal()
        {
            m_Sealed = true;
        }

        /// <summary>
        /// implements <see cref="ISealable.IsSealed"/>
        /// </summary>
        public bool IsSealed
        {
            get { return m_Sealed; }
        }

        #endregion

        /// <summary>
        /// A convenient method that throws <c>InstanceSealedException</c> when
        /// the instance is sealed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See documentation of the class <see cref="Sealable"/> for an example.
        /// </para>
        /// </remarks>
        /// <exception cref="InstanceSealedException">
        /// when instance is sealed
        /// </exception>
        protected void FailIfSealed()
        {
            if (m_Sealed) throw new InstanceSealedException();
        }

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> 
        /// <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override void Add(KeyValuePair<TKey, TValue> item)
        {
            FailIfSealed();
            base.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// 
        /// <exception cref="NotSupportedException">
        /// The <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> 
        /// <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override void Clear()
        {
            FailIfSealed();
            base.Clear();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// <remarks>
        /// return <see langword="true"/> when the wrapped collection returns 
        /// <c>true</c> or <see cref="IsSealed"/> is <see langword="true"/>.
        /// </remarks>
        /// <returns>
        /// true if the <see cref="ICollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly
        {
            get { return m_Sealed || WrappedInstance.IsReadOnly; }
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
        /// <param name="item">
        /// The object to remove from the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> 
        /// <see cref="IDictionary{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            FailIfSealed();
            return base.Remove(item);
        }

        #endregion


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
        /// The <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the 
        /// <see cref="IDictionary{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override void Add(TKey key, TValue value)
        {
            FailIfSealed();
            Wrapped.Add(key, value);
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
        /// The <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> <see cref="IDictionary{TKey, TValue}"/> 
        /// is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="InstanceSealedException">
        /// When <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        public override bool Remove(TKey key)
        {
            FailIfSealed();
            return Wrapped.Remove(key);
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> 
        /// <see cref="Dictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key is not found.
        /// </exception>
        /// <exception cref="InstanceSealedException">
        /// When the property is set and <see cref="IsSealed"/> is 
        /// <see langword="true"/>.
        /// </exception>
        public override TValue this[TKey key]
        {
            get
            {
                return Wrapped[key];
            }
            set
            {
                FailIfSealed();
                Wrapped[key] = value;
            }
        }

        #endregion


        #region ISealableDictionary Members

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        ///
        /// <remarks>
        /// Redefined indexer that has same signature as the indexers in both
        /// <see cref="IDictionary"/> and <see cref="IIndexable"/> to avoid 
        /// the ambiguous error in the implementing sub classes.
        /// </remarks>
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
        object ISealableDictionary.this[object key]
        {
            get
            {
                return base.NonGenericIndexerGet(key);
            }
            set
            {
                base.NonGenericIndexerSet(key, value);
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion
    }
}
