using System.Collections.Generic;
using Common.Collection;

namespace CodeSharp.Proxy.NPC
{
    class DictionaryProxy<TKey, TValue> : AbstractTransformingDictionary<TKey, TValue, TKey, TValue>
        where TValue : class 
    {
        public static IDictionary<TKey, TValue> GetProxy(IDictionary<TKey, TValue> targets)
        {
            return targets == null ? null : 
                (targets is DictionaryProxy<TKey, TValue> ? targets : new DictionaryProxy<TKey, TValue>(targets));
        }

        public static IDictionary<TKey, TValue> GetTarget(IDictionary<TKey, TValue> proxies)
        {
            var confirmProxy = proxies as DictionaryProxy<TKey, TValue>;
            return confirmProxy != null ? confirmProxy._source : proxies;
        }

        public DictionaryProxy(IDictionary<TKey, TValue> source) : base(source)
        {
        }

        protected sealed override TValue ReverseValue(TValue value)
        {
            return NotifyPropertyChangeFactory.GetTarget(value);
        }

        protected sealed override TKey TransformKey(TKey key)
        {
            return key;
        }

        protected sealed override TKey ReverseKey(TKey key)
        {
            return key;
        }

        protected sealed override TValue TransformValue(TValue value)
        {
            return NotifyPropertyChangeFactory.GetProxy(value);
        }

        public IDictionary<TKey, TValue> Target
        {
            get { return _source; }
        }
    }
}
