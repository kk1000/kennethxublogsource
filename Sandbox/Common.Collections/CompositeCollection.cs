using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// A collection that is consisted of a number of other collections.
    /// </summary>
    /// <typeparam name="T">type of the items that the collection contains</typeparam>
    /// <author>Kenneth Xu</author>
    public class CompositeCollection<T> : AbstractCollection<T>
    {
        private readonly List<ICollection<T>> _collections;

        #region Operator Overloading


        /// <summary>
        /// Add a collection <paramref name="c"/> to the <see cref="CompositeCollection{T}"/>
        /// <paramref name="cc"/>.
        /// </summary>
        /// <param name="cc">The <see cref="CompositeCollection{T}"/>.</param>
        /// <param name="c">The collection to be added.</param>
        /// <returns>The same <see cref="CompositeCollection{T}"/> <paramref name="cc"/></returns>
        public static CompositeCollection<T> operator 
            +(CompositeCollection<T> cc, ICollection<T> c)
        {
            cc.AddCollection(c);
            return cc;
        }

        /// <summary>
        /// Insert a collection <paramref name="c"/> to the beginning of the 
        /// <see cref="CompositeCollection{T}"/> <paramref name="cc"/>.
        /// </summary>
        /// <param name="cc">The <see cref="CompositeCollection{T}"/>.</param>
        /// <param name="c">The collection to be inserted.</param>
        /// <returns>The same <see cref="CompositeCollection{T}"/> <paramref name="cc"/></returns>
        public static CompositeCollection<T> operator 
            +(ICollection<T> c, CompositeCollection<T> cc)
        {
            cc.InsertCollection(0, c);
            return cc;
        }

        /// <summary>
        /// Add a range of collections to a <see cref="CompositeCollection{T}"/>.
        /// </summary>
        /// <param name="cc">A <see cref="CompositeCollection{T}"/>.</param>
        /// <param name="ec">A range of enumerators to be added.</param>
        /// <returns>The same <see cref="CompositeCollection{T}"/> <paramref name="cc"/></returns>
        public static CompositeCollection<T> operator 
            +(CompositeCollection<T> cc, IEnumerable<ICollection<T>> ec)
        {
            cc.AddCollections(ec);
            return cc;
        }

        /// <summary>
        /// Insert a range of collections to a <see cref="CompositeCollection{T}"/>.
        /// </summary>
        /// <param name="cc">A <see cref="CompositeCollection{T}"/>.</param>
        /// <param name="ec">A range of enumerators to be inserted.</param>
        /// <returns>The same <see cref="CompositeCollection{T}"/> <paramref name="cc"/></returns>
        public static CompositeCollection<T> operator 
            +(IEnumerable<ICollection<T>> ec, CompositeCollection<T> cc)
        {
            cc.InsertCollections(0, ec);
            return cc;
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Construct an empty collection that other collections can
        /// be added to it later.
        /// </summary>
        public CompositeCollection()
            : this(null, 0)
        {
        }

        /// <summary>
        /// Construct a collection with the given initial <paramref name="capacity"/>
        /// that other collections can be added to it later.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeCollection(int capacity)
            : this(null, capacity)
        {
        }


        /// <summary>
        /// Construct a collection that is composite of multiple 
        /// <paramref name="collections"/>. More collections can be added later.
        /// </summary>
        /// <param name="collections">
        /// Collections that this <see cref="CompositeCollection{T}"/> initially contains.
        /// </param>
        public CompositeCollection(IEnumerable<ICollection<T>> collections)
            : this(collections, 0)
        {
        }

        /// <summary>
        /// Construct a collection with the given initial <paramref name="capacity"/> 
        /// that is composite of multiple <paramref name="collections"/>. More 
        /// collections can be added to it later.
        /// </summary>
        /// <param name="collections">
        /// Collections that this <see cref="CompositeCollection{T}"/> initially contains.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the composition. Default initial capacity
        /// will be used when the value is less then or equals to zero.
        /// </param>
        public CompositeCollection(IEnumerable<ICollection<T>> collections, int capacity)
        {
            _collections = capacity > 0 ? 
                new List<ICollection<T>>(capacity) : 
                new List<ICollection<T>>();

            if (collections != null) AddCollections(collections);
        }

        #endregion

        /// <summary>
        /// Add the <paramref name="collection"/> to the end of composition.
        /// </summary>
        /// <param name="collection">
        /// The collection to be added to the end of the composition.
        /// </param>
        public void AddCollection(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            _collections.Add(collection);
        }

        /// <summary>
        /// Insert the <paramref name="collection"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="collection">The collection to be inserted.</param>
        public void InsertCollection(int position, ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            _collections.Insert(position, collection);
        }

        /// <summary>
        /// Add a range of <paramref name="collections"/> to the end of the composition.
        /// </summary>
        /// <param name="collections">
        /// The collections to be added to the end of the composition.
        /// </param>
        public void AddCollections(IEnumerable<ICollection<T>> collections)
        {
            if (collections == null) throw new ArgumentNullException("collections");
            EnsureNoNull(collections);
            _collections.AddRange(collections);
        }

        /// <summary>
        /// Insert a range of <paramref name="collections"/> to the given 
        /// <paramref name="position"/> of the composition.
        /// </summary>
        /// <param name="position">The position to insert. Starts from zero.</param>
        /// <param name="collections">The collections to be inserted.</param>
        public void InsertCollections(int position, IEnumerable<ICollection<T>> collections)
        {
            if (collections == null) throw new ArgumentNullException("collections");
            EnsureNoNull(collections);
            _collections.InsertRange(position, collections);
        }

        #region ICollection<T> Members

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value.
        /// </summary>
        /// <remarks>
        /// This implementation check all the collections that consists this composite
        /// collection to see if any of them contains the given <paramref name="item"/>.
        /// </remarks>
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(T item)
        {
            foreach (ICollection<T> c in _collections)
            {
                if (c.Contains(item)) return true;
            }
            return false;
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
        public override void CopyTo(T[] array, int arrayIndex)
        {
            foreach (ICollection<T> c in _collections)
            {
                c.CopyTo(array, arrayIndex);
                arrayIndex += c.Count;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <remarks>
        /// This implementation returns total count of all collections that consists
        /// this composite collection.
        /// </remarks>
        /// <returns>
        /// The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </returns>
        /// 
        public override int Count
        {
            get {
                int count = 0;
                foreach (ICollection<T> c in _collections)
                {
                    count += c.Count;
                }
                return count;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate 
        /// through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<T> GetEnumerator()
        {
            CompositeEnumerator<T> e = new CompositeEnumerator<T>(_collections.Count);
            foreach (ICollection<T> col in _collections)
            {
                e.Add(col.GetEnumerator());
            }
            return e;
        }

        #endregion

        #region ICollection Members

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
            foreach (ICollection<T> c in _collections)
            {
                foreach (T item in c) array.SetValue(item, index++);
            }
        }

        #endregion

        private static void EnsureNoNull(IEnumerable<ICollection<T>> e)
        {
            foreach (ICollection<T> ts in e)
            {
                if (ts==null) throw new NullReferenceException(
                    "Collection contains null element.");
            }
        }

    }
}
