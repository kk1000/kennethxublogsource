using System;
using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// Create an enumerator that can enumerate through multiple enumerators
    /// that are specified in the constructor.
    /// </summary>
    /// <remarks>
    /// The enumerators in the composition can only be specified once when the 
    /// instance is constructed. <see cref="CompositeEnumerator{T}"/> provides 
    /// much more flexibility but this class is more light weight.
    /// </remarks>
    /// <typeparam name="T">The type of the elements to enumerate through</typeparam>
    /// <seealso cref="CompositeEnumerator{T}"/>
    /// <author>Kenneth Xu</author>
    public class SimpleCompositeEnumerator<T> : AbstractCompositeEnumerator<T>
    {
        private readonly IEnumerable<IEnumerator<T>> _enumerators;

        /// <summary>
        /// Construct a composite enumerator that can iterate through elements
        /// in all given <paramref name="enumerators"/>.
        /// </summary>
        /// <param name="enumerators">
        /// Enumerators that will be part of the composition.
        /// </param>
        public SimpleCompositeEnumerator(IEnumerable<IEnumerator<T>> enumerators)
        {
            if (enumerators==null) throw new ArgumentNullException("enumerators");
            _enumerators = enumerators;
        }

        /// <summary>
        /// Construct a composite enumerator that can iterate through elements
        /// in all given <paramref name="enumerators"/>.
        /// </summary>
        /// <param name="enumerators">
        /// Enumerators that will be part of the composition.
        /// </param>
        public SimpleCompositeEnumerator(params IEnumerator<T>[] enumerators)
            :this((IEnumerable<IEnumerator<T>>)enumerators)
        {
        }

        /// <summary>
        /// Gets the enumerator to iterate through all compoisted enumerators.
        /// </summary>
        protected override IEnumerable<IEnumerator<T>> Enumerators
        {
            get { return _enumerators; }
        }
    }
}
