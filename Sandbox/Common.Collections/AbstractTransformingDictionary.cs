using System;
using System.Collections.Generic;

namespace Common.Collection
{
    public abstract class AbstractTransformingDictionary<KFrom, VFrom, KTo, VTo> : AbstractDictionary<KTo, VTo>
    {
        protected readonly IDictionary<KFrom, VFrom> _source;

        protected AbstractTransformingDictionary(IDictionary<KFrom, VFrom> source)
        {
            _source = source;
        }

        public override bool TryGetValue(KTo key, out VTo value)
        {
            VFrom result;
            var exists = _source.TryGetValue(ReverseKey(key), out result);
            value = exists ? TransformValue(result) : default(VTo);
            return exists;
        }

        public override IEnumerator<KeyValuePair<KTo, VTo>> GetEnumerator()
        {
            return new TransformingEnumerator<KeyValuePair<KFrom, VFrom>, KeyValuePair<KTo, VTo>>(
                _source.GetEnumerator(),
                p=>new KeyValuePair<KTo, VTo>(TransformKey(p.Key), TransformValue(p.Value)));
        }

        public override void Add(KTo key, VTo value)
        {
            _source.Add(ReverseKey(key), ReverseValue(value));
        }

        public override VTo this[KTo key]
        {
            get
            {
                return base[key];
            }
            set
            {
                _source[ReverseKey(key)] = ReverseValue(value);
            }
        }

        public override bool Remove(KTo key)
        {
            return _source.Remove(ReverseKey(key));
        }

        public override void Clear()
        {
            _source.Clear();
        }

        protected virtual VFrom ReverseValue(VTo value)
        {
            throw new NotSupportedException();
        }

        protected abstract KTo TransformKey(KFrom key);

        protected abstract KFrom ReverseKey(KTo key);

        protected abstract VTo TransformValue(VFrom value);
    }
}