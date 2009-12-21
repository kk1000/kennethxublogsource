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
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Collection
{
    /// <summary>
    /// This class consists exclusively of static methods that operate on or 
    /// return collections. It contains polymorphic algorithms that operate 
    /// on collections, "wrappers", which return a new collection backed by a 
    /// specified collection, and a few other odds and ends.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class Collections
    {
#if TODO
        /// <summary>
        /// Return a sealed view of the specified collection. Query 
        /// operations on the returned collection "read through" to the 
        /// specified collection, and attempts to modify the returned 
        /// collection, result in a <see cref="InstanceSealedException"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the element in the collection
        /// </typeparam>
        /// <param name="c">
        /// The collection for which a sealed view is to be return.
        /// </param>
        /// <returns>
        /// A sealed view of the specified collection.
        /// </returns>
        public static SealableCollection<T> Seal<T>(ICollection<T> c)
        {
            SealableCollection<T> sc = c is SealableCollection<T> ? 
                (SealableCollection<T>) c : new SealableCollection<T>(c);
            sc.Seal();
            return sc;
        }

        /// <summary>
        /// Return a sealed view of the specified dictionary. Query 
        /// operations on the returned dictionary "read through" to the 
        /// specified dictionary, and attempts to modify the returned 
        /// dictionary, result in a <see cref="InstanceSealedException"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the key in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Type of the type in the dictionary.
        /// </typeparam>
        /// <param name="d">
        /// The dictionary for which a sealed view is to be return.
        /// </param>
        /// <returns>
        /// A sealed view of the specified dictionary.
        /// </returns>
        public static SealableDictionary<TKey, TValue> Seal<TKey, TValue>(IDictionary<TKey, TValue> d)
        {
            SealableDictionary<TKey, TValue> sd = d is SealableDictionary<TKey, TValue> ?
                (SealableDictionary<TKey, TValue>)d : new SealableDictionary<TKey, TValue>(d);
            sd.Seal();
            return sd;
        }
        /// <summary>
        /// Return an unmodifiable view of the specified dictionary. This 
        /// method allows modules to provide users with "read-only" access to 
        /// internal dictionary. Query operations on the returned dictionary 
        /// "read through" to the specified dictionary, and attempts to modify 
        /// the returned dictionary, result in a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the key in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Type of the type in the dictionary.
        /// </typeparam>
        /// <param name="d">
        /// The dictionary for which an unmodifiable view is to be return.
        /// </param>
        /// <returns>
        /// A unmodifiable view of the specified dictionary.
        /// </returns>
        public static IDictionary<TKey, TValue> ReadOnly<TKey, TValue>(IDictionary<TKey, TValue> d)
        {
            return d.IsReadOnly ? d : Seal(d);
        }

        /// <summary>
        /// Return an unmodifiable view of the specified collection. This 
        /// method allows modules to provide users with "read-only" access to 
        /// internal collection. Query operations on the returned collection 
        /// "read through" to the specified collection, and attempts to modify 
        /// the returned collection, result in a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the element in the collection
        /// </typeparam>
        /// <param name="c">
        /// The collection for which an unmodifiable view is to be return.
        /// </param>
        /// <returns>
        /// An unmodifiable view of the specified collection.
        /// </returns>
        public static ICollection<T> ReadOnly<T>(ICollection<T> c)
        {
            return c.IsReadOnly ? c : Seal(c);
        }
#endif

        /// <summary>
        /// Copy elements in the enumerator <paramref name="e"/> to the 
        /// specified <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array to hold the elements.</param>
        /// <param name="arrayIndex">
        /// The index of the array to copy the first element.
        /// </param>
        /// <param name="e">To enumerate the elements to be copied.</param>
        public static void CopyTo<T>(T[] array, int arrayIndex, IEnumerator<T> e)
        {
            if (array == null) throw new ArgumentNullException("array");
            while (e.MoveNext())
            {
                array[arrayIndex++] = e.Current;
            }
        }

        /// <summary>
        /// Copy elements in the emunerator <paramref name="e"/> to the
        /// specified <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to hold the elements.</param>
        /// <param name="arrayIndex">
        /// The index of the array to copy the first element.
        /// </param>
        /// <param name="e">To emunerate the elements to be copied.</param>
        public static void CopyTo(Array array, int arrayIndex, IEnumerator e)
        {
            if (array == null) throw new ArgumentNullException("array");
            while(e.MoveNext())
            {
                array.SetValue(e.Current, arrayIndex++);
            }
        }

#pragma warning disable 1591
        public static void ReadXmlToDictionary<TKey, TValue>(XmlReader reader, IDictionary<TKey, TValue> dictionary)
        {
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return;
            }

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            reader.Read();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement) {

                reader.ReadStartElement(ITEM_ELEMENT_NAME); 

                TKey key = (TKey) keySerializer.Deserialize(reader); 
                TValue value = (TValue) valueSerializer.Deserialize(reader); 
                dictionary.Add(key, value); 

                reader.ReadEndElement(); // end of item
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public static void ReadXmlToCollection<T>(XmlReader reader, ICollection<T> collection)
        {
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return;
            }

            XmlSerializer valueSerializer = new XmlSerializer(typeof(T));
            reader.Read();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement)
            {

                T value = (T)valueSerializer.Deserialize(reader);
                collection.Add(value);
                reader.MoveToContent();

            }
            reader.ReadEndElement();
        }

        public static int ReadCollectionCountFromXml(XmlReader reader)
        {
            string s = reader.GetAttribute(COUNT_ATTRIBUTE_NAME);
            try
            {
                return Convert.ToInt32(s);
            }
            catch (FormatException e)
            {
                throw new XmlException(
                    "value of attribute '" + COUNT_ATTRIBUTE_NAME + "'" + 
                    " must be an integer, but encountered '" + s + "'", 
                    e);
            }
        }

        public static void WriteDictionaryToXml<TKey, TValue>(XmlWriter writer, IDictionary<TKey, TValue> dictionary)
        {
            writer.WriteStartAttribute(COUNT_ATTRIBUTE_NAME);
            writer.WriteValue(dictionary==null ? -1 : dictionary.Count);
            writer.WriteEndAttribute();

            if (dictionary == null) return;

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));//, KEY_ELEMENT);
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));//, VALUE_ELEMENT);
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                writer.WriteStartElement(ITEM_ELEMENT_NAME);
                keySerializer.Serialize(writer, pair.Key);
                valueSerializer.Serialize(writer, pair.Value);
                writer.WriteEndElement();
            }

        }

        public static void WriteCollectionToXml<T>(XmlWriter writer, ICollection<T> collection)
        {
            writer.WriteStartAttribute(COUNT_ATTRIBUTE_NAME);
            writer.WriteValue(collection == null ? -1 : collection.Count);
            writer.WriteEndAttribute();

            if (collection == null) return;

            XmlSerializer valueSerializer = new XmlSerializer(typeof(T));
            foreach (T value in collection)
            {
                valueSerializer.Serialize(writer, value);
            }

        }

        public static StringBuilder ToString(IEnumerator e)
        {
            return ToString(e, null);
        }

        public static StringBuilder ToString(IEnumerator e, StringBuilder sb)
        {
            if (sb == null) sb = new StringBuilder();
            bool first = true;
            while(e.MoveNext())
            {
                if (!first) sb.Append(", ");
                sb.Append(e.Current);
                first = false;
            }
            return sb;
        }

        public static StringBuilder ToString(IEnumerable e)
        {
            return ToString(e, null);
        }

        public static StringBuilder ToString(IEnumerable e, StringBuilder sb)
        {
            return ToString(e.GetEnumerator(), sb);
        }

        public static StringBuilder ToString<T>(IEnumerable<T> e)
        {
            return ToString(e, null);
        }

        public static StringBuilder ToString<T>(IEnumerable<T> e, StringBuilder sb)
        {
            return ToString(e.GetEnumerator(), sb);
        }

        public static StringBuilder ToString(IDictionary d)
        {
            return ToString(d, null);
        }

        public static StringBuilder ToString(IDictionary d, StringBuilder sb)
        {
            if (sb == null) sb = new StringBuilder();
            bool first = true;
            foreach (object key in d.Keys)
            {
                if (!first) sb.Append(", ");
                sb.Append(key).Append("=").Append(d[key]);
                first = false;
            }
            return sb;
        }

        public static StringBuilder ToString<K,V>(IDictionary<K,V> d)
        {
            return ToString(d, null);
        }

        public static StringBuilder ToString<K, V>(IDictionary<K, V> d, StringBuilder sb)
        {
            if (sb == null) sb = new StringBuilder();
            bool first = true;
            foreach (KeyValuePair<K,V> p in d)
            {
                if (!first) sb.Append(", ");
                sb.Append(p.Key).Append("=").Append(p.Value);
                first = false;
            }
            return sb;
        }
#pragma warning restore 1591

        /// <summary>
        /// Returns an immutable list containing only the specified object. 
        /// </summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="o">The sole object to be stored in the returned list.</param>
        /// <returns>An immutable list containing only the specified object.</returns>
        public static IList<T> SingletonList<T>(T o)
        {
            return new SingletonList<T>(o);
        }

        /// <summary>
        /// Returns an enumerable that can iterate through all the elements in 
        /// the given <paramref name="enumerable"/> that pass the given 
        /// <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumerable">The source enumerable to search.</param>
        /// <param name="predicate">The search criteria.</param>
        /// <returns>The enumerable for elements found.</returns>
        public static IEnumerable<T> FindAll<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");
            foreach (T item in enumerable)
            {
                if (predicate(item)) yield return item;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all the elements in 
        /// the given <paramref name="enumerator"/> that pass the given 
        /// <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumerator">The source enumerable to search.</param>
        /// <param name="predicate">The search criteria.</param>
        /// <returns>The enumerator for elements found.</returns>
        public static IEnumerator<T> FindAll<T>(IEnumerator<T> enumerator, Predicate<T> predicate)
        {
            if (enumerator == null) throw new ArgumentNullException("enumerator");
            while(enumerator.MoveNext())
            {
                T item = enumerator.Current;
                if (predicate(item)) yield return item;
            }
        }

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TBase"/> that is 
        /// backed by the specified enumerable of <typeparamref name="TSub"/>.
        /// </summary>
        /// <typeparam name="TSub">The type source enumerable.</typeparam>
        /// <typeparam name="TBase">The type of result enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <returns>An enumerable of the specified base type.</returns>
        public static IEnumerable<TBase> UpCast<TSub, TBase>(IEnumerable<TSub> source)
            where TSub : TBase
        {
            foreach (TSub item in source) yield return item;
        }

        private static void SwapIfGreaterWithItems<T, TValue>(IList<T> keys, IList<TValue> values, IComparer<T> comparer, int a, int b)
        {
            if (a != b)
            {
                try
                {
                    if (comparer.Compare(keys[a], keys[b]) > 0)
                    {
                        T local = keys[a];
                        keys[a] = keys[b];
                        keys[b] = local;
                        if (values != null)
                        {
                            TValue local2 = values[a];
                            values[a] = values[b];
                            values[b] = local2;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException(GetResourceString("Arg_BogusIComparer", new object[] { keys[b], keys[b].GetType().Name, comparer }));
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(GetResourceString("InvalidOperation_IComparerFailed"), exception);
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
        internal static extern string GetResourceFromDefault(string key);

        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceFromDefault = GetResourceFromDefault(key);
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, resourceFromDefault, values);
        }

        private static void QuickSort<T, TValue>(IList<T> keys, IList<TValue> values, int left, int right, IComparer<T> comparer)
        {
            do
            {
                int a = left;
                int b = right;
                int num3 = a + ((b - a) >> 1);
                SwapIfGreaterWithItems(keys, values, comparer, a, num3);
                SwapIfGreaterWithItems(keys, values, comparer, a, b);
                SwapIfGreaterWithItems(keys, values, comparer, num3, b);
                T y = keys[num3];
                do
                {
                    try
                    {
                        while (comparer.Compare(keys[a], y) < 0)
                        {
                            a++;
                        }
                        while (comparer.Compare(y, keys[b]) < 0)
                        {
                            b--;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException(GetResourceString("Arg_BogusIComparer", new object[] { y, y.GetType().Name, comparer }));
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException(GetResourceString("InvalidOperation_IComparerFailed"), exception);
                    }
                    if (a > b)
                    {
                        break;
                    }
                    if (a < b)
                    {
                        T local2 = keys[a];
                        keys[a] = keys[b];
                        keys[b] = local2;
                        if (values != null)
                        {
                            TValue local3 = values[a];
                            values[a] = values[b];
                            values[b] = local3;
                        }
                    }
                    a++;
                    b--;
                }
                while (a <= b);
                if ((b - left) <= (right - a))
                {
                    if (left < b)
                    {
                        QuickSort(keys, values, left, b, comparer);
                    }
                    left = a;
                }
                else
                {
                    if (a < right)
                    {
                        QuickSort(keys, values, a, right, comparer);
                    }
                    right = b;
                }
            }
            while (left < right);
        }



        private const string ITEM_ELEMENT_NAME = "item";
        private const string COUNT_ATTRIBUTE_NAME = "count";

    }
}
