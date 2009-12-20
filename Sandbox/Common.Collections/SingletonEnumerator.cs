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

namespace Common.Collection
{
    /// <summary>
    /// Enumerator for a single element.
    /// </summary>
    /// <typeparam name="T">Type of the elements to be iterated.</typeparam>
    /// <author>Kenneth Xu</author>
    public class SingletonEnumerator<T> : AbstractEnumerator<T>
    {
        private readonly T _theSingleton;

        /// <summary>
        /// Construct an emuerator for the single item <paramref name="o"/>.
        /// </summary>
        /// <param name="o">The only elemnt of the enumerator.</param>
        public SingletonEnumerator(T o)
        {
            _theSingleton = o;
        }

        /// <summary>
        /// Fetch the current element of the enumerator.
        /// </summary>
        /// <returns>The current element</returns>
        protected override T FetchCurrent()
        {
            return _theSingleton;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        ///
        /// <returns>
        /// true if the enumerator was successfully advanced to the next 
        /// element; false if the enumerator has passed the end of the collection.
        /// </returns>
        protected override bool GoNext()
        {
            return (State == EnumeratorState.BeforeStart);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before 
        /// the first element in the collection.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Reset()
        {
            State = EnumeratorState.BeforeStart;
        }

    }
}
