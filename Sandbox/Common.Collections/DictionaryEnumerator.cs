#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
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
using System.Collections;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// Class that wraps the generic enumerator as <see cref="IDictionaryEnumerator"/>.
    /// </summary>
    /// <typeparam name="Tkey">Type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the value of the dictionary.</typeparam>
    /// <author>Kenneth Xu</author>
    public class DictionaryEnumerator<Tkey, TValue> : IDictionaryEnumerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">
        /// The generic <see cref="IEnumerator{T}"/> of 
        /// <see cref="KeyValuePair{TKey,TValue}">KeyValuePair of TKey and TValue</see>
        /// to be wrapped as <see cref="IDictionaryEnumerator"/>.
        /// </param>
        public DictionaryEnumerator(IEnumerator<KeyValuePair<Tkey, TValue>> e)
        {
            if (e == null) throw new ArgumentNullException("e");
            m_wrapped = e;
        }

        #region IDictionaryEnumerator Members

        ///<summary>
        ///Gets both the key and the value of the current dictionary entry.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.DictionaryEntry"></see> containing 
        /// both the key and the value of the current dictionary entry.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Collections.IDictionaryEnumerator"></see> 
        /// is positioned before the first entry of the dictionary or after the 
        /// last entry. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public DictionaryEntry Entry
        {
            get { return new DictionaryEntry(Key, Value); }
        }

        ///<summary>
        ///Gets the key of the current dictionary entry.
        ///</summary>
        ///
        ///<returns>
        ///The key of the current element of the enumeration.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Collections.IDictionaryEnumerator"></see> 
        /// is positioned before the first entry of the dictionary or after 
        /// the last entry. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object Key
        {
            get { return m_wrapped.Current.Key; }
        }

        ///<summary>
        ///Gets the value of the current dictionary entry.
        ///</summary>
        ///
        ///<returns>
        ///The value of the current element of the enumeration.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Collections.IDictionaryEnumerator"></see> 
        /// is positioned before the first entry of the dictionary or after 
        /// the last entry. </exception>
        /// <filterpriority>2</filterpriority>
        public object Value
        {
            get { return m_wrapped.Current.Value; }
        }

        #endregion

        #region IEnumerator Members

        ///<summary>
        ///Gets the current element in the collection.
        ///</summary>
        ///
        ///<returns>
        ///The current element in the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The 
        /// enumerator is positioned before the first element of the 
        /// collection or after the last element. </exception>
        /// <filterpriority>2</filterpriority>
        public object Current
        {
            get { return Entry; }
        }

        ///<summary>
        ///Advances the enumerator to the next element of the collection.
        ///</summary>
        ///
        ///<returns>
        ///true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the 
        /// collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection 
        /// was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            return m_wrapped.MoveNext();
        }

        ///<summary>
        ///Sets the enumerator to its initial position, which is before the 
        /// first element in the collection.
        ///</summary>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection 
        /// was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public void Reset()
        {
            m_wrapped.Reset();
        }

        #endregion

        #region Private Instance Fields

        private readonly IEnumerator<KeyValuePair<Tkey, TValue>> m_wrapped;

        #endregion

        ///<summary>
        ///Serves as a hash function for a particular type. 
        /// <see cref="M:System.Object.GetHashCode"></see> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        ///</summary>
        ///
        ///<returns>
        ///A hash code for the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return m_wrapped.GetHashCode();
        }

        ///<summary>
        ///Determines whether the specified <see cref="T:System.Object"></see> 
        /// is equal to the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///true if the specified <see cref="T:System.Object"></see> is equal to 
        /// the current <see cref="T:System.Object"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="obj">The <see cref="T:System.Object"></see> to compare 
        /// with the current <see cref="T:System.Object"></see>. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            DictionaryEnumerator<Tkey, TValue> e =
                obj as DictionaryEnumerator<Tkey, TValue>;
            return e != null && m_wrapped.Equals(e.m_wrapped);
        }
    }
}
