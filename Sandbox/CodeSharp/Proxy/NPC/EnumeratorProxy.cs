using System.Collections.Generic;
using Common.Collection;

namespace CodeSharp.Proxy.NPC
{
    internal class EnumeratorProxy<T> : AbstractTransformingEnumerator<T, T>
        where T : class 
    {
        public static IEnumerator<T> GetProxy(IEnumerator<T> targets)
        {
            return targets == null ? null : 
                (targets is EnumeratorProxy<T> ? targets : new EnumeratorProxy<T>(targets));
        }

        public static IEnumerator<T> GetTarget(IEnumerator<T> proxies)
        {
            var confirmProxy = proxies as EnumeratorProxy<T>;
            return confirmProxy != null ? confirmProxy._source : proxies;
        }

        public EnumeratorProxy(IEnumerator<T> source) : base(source)
        {
        }

        protected override T Transform(T source)
        {
            return NotifyPropertyChangedFactory.GetProxy(source);
        }

        public IEnumerator<T> Target
        {
            get { return _source; }
        }
    }
}