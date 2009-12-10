using System.Collections.Generic;

namespace System.Extension
{
    public class Indexable<TKey, TValue> : IIndexable<TKey, TValue>, IIndexable
    {
        private IDictionary<TKey, TValue> storage;

        public Indexable(){}


        #region IIndexable<TKey,TValue> Members

        public TValue this[TKey index]
        {
            get
            {
                return storage[index];
            }
            set
            {
                storage[index] = value;
            }
        }

        #endregion

        #region IIndexable Members

        public object this[object index]
        {
            get
            {
                return this[(TKey)index];
            }
            set
            {
                this[(TKey)index] = (TValue)value;
            }
        }

        #endregion
    }
}
