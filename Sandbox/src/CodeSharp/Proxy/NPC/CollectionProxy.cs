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

using System.Collections.Generic;
using Common.Collection;

namespace CodeSharp.Proxy.NPC
{
    /// <author>Kenneth Xu</author>
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
