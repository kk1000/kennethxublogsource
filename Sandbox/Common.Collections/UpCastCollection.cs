using System.Collections.Generic;
using System;

namespace Common.Collection
{
    /// <summary>
    /// Transforms a <see cref="ICollection{T}">ICollection(Of TSub)</see>
    /// to a <see cref="ICollection{T}">ICollection(Of TBase)</see> by
    /// simply casting the elements when <typeparamref name="TSub"/> is a
    /// derived type of <typeparamref name="TBase"/>.
    /// </summary>
    /// <typeparam name="TSub">The sub type.</typeparam>
    /// <typeparam name="TBase">The base type.</typeparam>
    public class UpCastCollection<TSub, TBase> : AbstractTransformingCollection<TSub, TBase>
        where TSub : TBase
    {
        #region Constructors

        /// <summary>
        /// Construct a collection of <typeparamref name="TBase"/> by 
        /// transforming the elemnts from <paramref name="source"/>.
        /// </summary>
        /// <remarks>
        /// The resulting collection is mutable by down casting 
        /// <typeparamref name="TBase"/> to <typeparamref name="TSub"/>
        /// when necessary.
        /// </remarks>
        /// <param name="source">
        /// The source collection of <typeparamref name="TSub"/>.
        /// </param>
        public UpCastCollection(ICollection<TSub> source)
            : base(source)
        {
        }

        #endregion

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific 
        /// value. 
        /// </summary>
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// 
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        public override bool Contains(TBase item)
        {
            return item is TSub && SourceCollection.Contains((TSub)item);
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
        /// When the <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public override bool Remove(TBase item)
        {
            return item is TSub && SourceCollection.Remove((TSub)item);
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>. This implementation
        /// throw <see cref="InvalidCastException"/> if <paramref name="item"/>
        /// is not a <typeparamref name="TSub"/>.
        /// </summary>
        /// 
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// When <paramref name="item"/> is not of type <typeparamref name="TSub"/>.
        /// </exception>
        public override void Add(TBase item)
        {
            SourceCollection.Add((TSub)item);
        }

        /// <summary>
        /// No-op tranformation, simply return the <paramref name="source"/>
        /// as is.
        /// </summary>
        /// <param name="source">
        /// Instace of <typeparamref name="TSub"/> to be upcasted.
        /// </param>
        /// <returns>
        /// The same instance of <paramref name="source"/>.
        /// </returns>
        protected override TBase Transform(TSub source)
        {
            return source;
        }

        /// <summary>
        /// Converts object of type <typeparamref name="TBase"/> to
        /// <typeparamref name="TSub"/>.
        /// </summary>
        /// <remarks>
        /// This implementation always return <c>false</c>. Subclasses that
        /// support reversing should override this method.
        /// </remarks>
        /// <param name="target">
        /// Instance of <typeparamref name="TBase"/> to be converted.
        /// </param>
        /// <param name="source">
        /// Converted object of type <typeparamref name="TSub"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> when reserving is supported, otherwise <c>false</c>.
        /// </returns>
        protected override bool TryReverse(TBase target, out TSub source)
        {
            source = (TSub) target;
            return true;
        }

        #region Private Instance Fields
        #endregion
    }
}
