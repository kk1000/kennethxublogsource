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
