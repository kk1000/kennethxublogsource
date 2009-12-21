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
    /// <author>Kenneth Xu</author>
    internal class SingletonList<T> : AbstractList<T>
    {
        private T _theOne;

        public SingletonList(T o)
        {
            _theOne = o;
        }


        #region ICollection<T> Members

        public override bool Contains(T item)
        {
            return ReferenceEquals(_theOne, item) || _theOne.Equals(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            array[arrayIndex] = _theOne;
        }

        public override int Count
        {
            get { return 1; }
        }


        #endregion

        #region IEnumerable<T> Members

        public override IEnumerator<T> GetEnumerator()
        {
            return new SingletonEnumerator<T>(_theOne);
        }

        #endregion


        #region ICollection Members

        protected override void CopyTo(Array array, int index)
        {
            array.SetValue(_theOne, index);
        }

        protected override bool IsSynchronized
        {
            get { return true; }
        }

        protected override object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IList<T> Members

        public override int IndexOf(T item)
        {
            return Contains(item) ? 0 : -1;
        }

        public override T this[int index]
        {
            get
            {
                if (index == 0) return _theOne;
                else throw new ArgumentOutOfRangeException(
                    "index", index, "list has only one element.");
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region IList Members

        protected override bool IsFixedSize
        {
            get { return true; }
        }

        #endregion
    }
}
