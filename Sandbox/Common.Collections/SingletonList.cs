using System;
using System.Collections.Generic;

namespace Common.Collection
{
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