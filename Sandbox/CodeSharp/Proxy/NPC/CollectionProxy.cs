using System.Collections.Generic;
using Common.Collection;

namespace CodeSharp.Proxy.NPC
{
    class CollectionProxy<T> : AbstractTransformingCollection<T, T>
        where T: class 
    {
        public static ICollection<T> GetProxy(ICollection<T> targets)
        {
            return targets == null ? null : 
                (targets is CollectionProxy<T> ? targets : new CollectionProxy<T>(targets));
        }

        public static ICollection<T> GetTarget(ICollection<T> proxies)
        {
            var confirmProxy = proxies as CollectionProxy<T>;
            return confirmProxy != null ? confirmProxy.SourceCollection : proxies;
        }

        public CollectionProxy(ICollection<T> source) : base(source)
        {
        }

        protected override bool TryReverse(T target, out T source)
        {
            source = NotifyPropertyChangeFactory.GetTarget(target);
            return true;
        }

        protected override T Transform(T source)
        {
            return NotifyPropertyChangeFactory.GetProxy(source);
        }
    }
}