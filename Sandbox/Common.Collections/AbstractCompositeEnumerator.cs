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
    /// Convenient base class to simplify the implementation the composite
    /// enumerators.
    /// </summary>
    /// <typeparam name="T">Type of the elements to be iterated.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractCompositeEnumerator<T> : AbstractEnumerator<T>
    {
        private IEnumerator<IEnumerator<T>> _eoe; // enumerator of enumerators

        /// <summary>
        /// Gets the enumerator to iterate through all compoisted enumerators.
        /// </summary>
        protected abstract IEnumerable<IEnumerator<T>> Enumerators { get; }

        /// <summary>
        /// Fetch the current element of the enumerator.
        /// </summary>
        /// <returns>The current element</returns>
        protected override T FetchCurrent()
        {
            return _eoe.Current.Current;
        }

        /// <summary>
        /// Disposes all composited enumerators.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (_eoe != null)
            {
                _eoe.Dispose();
                _eoe = null;
            }
            foreach (IEnumerator<T> e in Enumerators)
            {
                if (e!=null) 
                    try {e.Dispose();} catch {} // ignore error.
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        ///
        /// <returns>
        /// true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the collection.
        /// </returns>
        ///
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        protected override bool GoNext()
        {
            bool _hasMore = true;

            if (_eoe == null)
            {
                _eoe = Enumerators.GetEnumerator();
                _hasMore = _eoe.MoveNext();
            }
            while(_hasMore)
            {
                if (_eoe.Current!=null && _eoe.Current.MoveNext()) return true;
                else _hasMore = _eoe.MoveNext();
            }
            return false;

        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before 
        /// the first element in the collection.
        /// </summary>
        /// <remarks>
        /// Resets all composited enumerators.
        /// </remarks>
        /// <exception cref="T:System.InvalidOperationException">
        /// Any composited enumerator throws the <see cref="InvalidOperationException"/>.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override void Reset()
        {
            _eoe = null;
            foreach (IEnumerator<T> e in Enumerators)
            {
                if (e!=null) e.Reset();
            }
        }

    }
}