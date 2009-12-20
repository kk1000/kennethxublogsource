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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Collection
{
    /// <author>Kenneth Xu</author>
    public class TransformingDictionary<KFrom, VFrom, KTo, VTo> : AbstractTransformingDictionary<KFrom, VFrom, KTo, VTo>
    {
        private readonly Converter<KFrom, KTo> _keyTransformer;
        private readonly Converter<KTo, KFrom> _keyReverser;
        private readonly Converter<VFrom, VTo> _valueTransformer;
        private readonly Converter<VTo, VFrom> _valueReverser;

        public TransformingDictionary(
            IDictionary<KFrom, VFrom> source,
            Converter<KFrom, KTo> keyTransformer,
            Converter<KTo, KFrom> keyReverser,
            Converter<VFrom, VTo> valueTransformer,
            Converter<VTo, VFrom> valueReverser) 
            : base(source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keyTransformer == null) throw new ArgumentNullException("keyTransformer");
            if (keyReverser == null) throw new ArgumentNullException("keyReverser");
            if (valueTransformer == null) throw new ArgumentNullException("valueTransformer");
            _keyTransformer = keyTransformer;
            _keyReverser = keyReverser;
            _valueTransformer = valueTransformer;
            _valueReverser = valueReverser;
        }

        protected override KTo TransformKey(KFrom key)
        {
            return _keyTransformer(key);
        }

        protected override KFrom ReverseKey(KTo key)
        {
            return _keyReverser(key);
        }

        protected override VTo TransformValue(VFrom value)
        {
            return _valueTransformer(value);
        }

        protected override VFrom ReverseValue(VTo value)
        {
            return _valueReverser == null ? base.ReverseValue(value) : _valueReverser(value);
        }
    }

    public class TransformingDictionary<TKey, VFrom, VTo> : AbstractTransformingDictionary<TKey, VFrom, TKey, VTo>
    {
        private readonly Converter<VFrom, VTo> _transformer;
        private readonly Converter<VTo, VFrom> _reverser;

        public TransformingDictionary(
            IDictionary<TKey, VFrom> source,
            Converter<VFrom, VTo> transformer,
            Converter<VTo, VFrom> reverser)
            : base(source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (transformer == null) throw new ArgumentNullException("transformer");
            _transformer = transformer;
            _reverser = reverser;
        }

        protected sealed override TKey TransformKey(TKey key)
        {
            return key;
        }

        protected sealed override TKey ReverseKey(TKey key)
        {
            return key;
        }

        protected sealed override VTo TransformValue(VFrom value)
        {
            return _transformer(value);
        }

        protected sealed override VFrom ReverseValue(VTo value)
        {
            return _reverser == null ? base.ReverseValue(value) : _reverser(value);
        }
    }
}
