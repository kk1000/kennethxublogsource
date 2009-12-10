using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeSharp.Proxy.NPC
{
    class EnumerableProxy<T> : IEnumerable<T>
        where T : class 
    {
        private readonly IEnumerable<T> _source;
        public static IEnumerable<T> GetProxy(IEnumerable<T> targets)
        {
            return targets == null ? null : 
                (targets is EnumerableProxy<T> ? targets : new EnumerableProxy<T>(targets));
        }

        public static IEnumerable<T> GetTarget(IEnumerable<T> proxies)
        {
            var confirmProxy = proxies as EnumerableProxy<T>;
            return confirmProxy != null ? confirmProxy._source : proxies;
        }

        public EnumerableProxy(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorProxy<T>(_source.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Target
        {
            get { return _source; }
        }
    }
}
