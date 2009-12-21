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
