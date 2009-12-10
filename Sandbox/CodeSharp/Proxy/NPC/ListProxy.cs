using System.Collections.Generic;
using Common.Collection;

namespace CodeSharp.Proxy.NPC
{
    class ListProxy<T> : AbstractTransformingList<T, T>
        where T : class 
    {
        public static IList<T> GetProxy(IList<T> targets)
        {
            return targets == null ? null : 
                (targets is ListProxy<T> ? targets : new ListProxy<T>(targets));
        }

        public static IList<T> GetTarget(IList<T> proxies)
        {
            var confirmProxy = proxies as ListProxy<T>;
            return confirmProxy != null ? confirmProxy.SourceList : proxies;
        }
        public ListProxy(IList<T> source) : base(source)
        {
        }

        protected override T Reverse(T target)
        {
            return NotifyPropertyChangedFactory.GetTarget(target);
        }

        protected override T Transform(T source)
        {
            return NotifyPropertyChangedFactory.GetProxy(source);
        }
    }
}
