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
using System.Collections;
using System.Collections.Generic;

namespace CodeSharp.Proxy.NPC
{
    /// <author>Kenneth Xu</author>
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
