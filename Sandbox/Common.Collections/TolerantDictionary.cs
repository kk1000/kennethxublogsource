using System;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// A generic dictionary that won't throw <see cref="KeyNotFoundException"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically, a generic dictionary will throw <see cref="KeyNotFoundException"/>
    /// when the indexer is called with a key that doesn't exist in the dictionary.
    /// </para>
    /// <example>
    /// Example below will throw the <c>KeyNotFoundException</c>
    /// <code language="C#">
    ///   IDictionary&lt;string, long&gt; d = new Dictionary&lt;string, long&gt;();
    ///   long value = d["nosuchkey"]; // KeyNotFoundException
    /// </code>
    /// </example>
    /// <para>
    /// In many real use cases, it would be much appreciated if a default value 
    /// is returned when a key is not found. While the <see cref="IDictionary{K,V}.TryGetValue"/>
    /// method is available for this purpose, but it is cumbersome to write and
    /// often a dictionary needs to be passed to 3rd party library that was written
    /// to use the indexer instead of <c>TryGetValue</c>.
    /// </para>
    /// <para>
    /// This class is a thin wrapper designed to overcome this problem. It optionally
    /// accecpts another dictionary and a deault value in its constructors, and makes
    /// sure that the indexer will return the default value when a key is not exist.
    /// </para>
    /// <example>
    /// Example below returns <c>-1</c> instead of throwing the <c>KeyNotFoundException</c>
    /// <code language="C#">
    ///   IDictionary&lt;string, long&gt; d = new TolerantDictionary&lt;string, long&gt;(-1);
    ///   long value = d["nosuchkey"]; // returns -1;
    /// </code>
    /// </example>
    /// </remarks>
    /// <typeparam name="TKey">dictinarly key type</typeparam>
    /// <typeparam name="TValue">dictionary value type</typeparam>
    /// <author>Kenneth Xu</author>
    public class TolerantDictionary<TKey, TValue> : DictionaryWrapper<TKey, TValue>
    {
        private readonly TValue _default;

        #region Constructors

        /// <summary>
        /// Construct a dictionary behaves the same as <see cref="Dictionary{TKey,TValue}"/>
        /// except that its indexer returns the default value of type 
        /// <typeparamref name="TValue"/> when a key doesn't exist in the dictioanry.
        /// </summary>
        public TolerantDictionary()
            : base(new Dictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Construct a dictionary behaves the same as <see cref="Dictionary{K,V}"/>
        /// except that its indexer returns the <paramref name="defaultValue"/>
        /// when a key doesn't exist in the dictioanry.
        /// </summary>
        /// <param name="defaultValue">
        /// The value to return when a key doesn't exist
        /// </param>
        public TolerantDictionary(TValue defaultValue) : this() 
        {
            _default = defaultValue;
        }

        /// <summary>
        /// Construct a dictionary that wraps another <paramref name="dictionary"/>.
        /// It delegates all the operations to the wrapped <c>dictionary</c>
        /// except that its indexer returns default value of type 
        /// <typeparamref name="TValue"/> when a key doesn't exist in the dictioanry.
        /// </summary>
        /// <param name="dictionary">
        /// the dictionary to be dictionary as tolerant when a key passed to the indexer
        /// is not found.
        /// </param>
        public TolerantDictionary(IDictionary<TKey, TValue> dictionary) 
            : base(dictionary)
        {
        }

        /// <summary>
        /// Construct a dictionary that wraps another <paramref name="dictionary"/>. 
        /// It delegates all the operations to the wrapped <c>dictionary</c> 
        /// except that its indexer returns the <paramref name="defaultValue"/> 
        /// when a key doesn't exist in the dictioanry.
        /// </summary>
        /// <param name="dictionary">
        /// the dictionary to be dictionary as tolerant when a key passed to the indexer
        /// is not found.
        /// </param>
        /// <param name="defaultValue">
        /// The value to return when a key doesn't exist
        /// </param>
        public TolerantDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
            : base(dictionary)
        {
            _default = defaultValue;
        }

        #endregion

        /// <summary>
        /// Readonly property to get the value that will be returned by the indexer 
        /// if a key is not found. This value can only be set in the constructors.
        /// </summary>
        public TValue Default
        {
            get { return _default; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <remarks>
        /// Dictionary indexer that will never throw <see cref="KeyNotFoundException"/>.
        /// Instead, it returns the <see cref="Default"/> value if a index is not found.
        /// </remarks>
        /// <returns>
        /// The element with the specified key, or value of the <see cref="Default"/>
        /// property if the key doesn't exist in dictionary.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="AbstractDictionaryWrapper{TKey,TValue}.Wrapped"/> 
        /// <see cref="Dictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public override TValue this[TKey key]
        {
            get
            {
                TValue value;
                return TryGetValue(key, out value) ? value : _default;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
