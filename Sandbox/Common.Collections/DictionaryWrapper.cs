using System;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// <see cref="DictionaryWrapper{TKey, TValue}"/> is a plain wrapper 
    /// which does nothing but delegate all memeber access to the wrapped 
    /// <see cref="IDictionary{TKey,TValue}"/> instance.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class DictionaryWrapper<TKey, TValue> : AbstractDictionaryWrapper<TKey, TValue>
    {
        /// <summary>
        /// Construct a new instance of <see cref="DictionaryWrapper{TKey,TValue}"/>
        /// that wraps the given <paramref name="dictionary"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary to be wrapped.</param>
        /// <exception cref="ArgumentNullException">
        /// When paremeter <paramref name="dictionary"/> is <see langword="null"/>.
        /// </exception>
        public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            WrappedInstance = dictionary;
        }

        /// <summary>
        /// Gets the wrapped instance, which is <see cref="WrappedInstance"/>.
        /// </summary>
        protected override IDictionary<TKey, TValue> Wrapped
        {
            get { return WrappedInstance; }
        }

        /// <summary>
        /// The wrapped dictionary instance.
        /// </summary>
        protected IDictionary<TKey, TValue> WrappedInstance;
    }
}
