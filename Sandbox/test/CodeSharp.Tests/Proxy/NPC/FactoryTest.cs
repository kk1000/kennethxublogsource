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
using NUnit.Framework;

namespace CodeSharp.Proxy.NPC
{
    /// <author>Kenneth Xu</author>
    public class FactoryTest
    {
        [Test]
        public void SetBaseTypeChokesOnValueType()
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(ValueType), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test]
        public void SetBaseTypeChokesOnTypeDoesNotImplementINotifyPropertyChanged()
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(NotNotifyPropertyChanged), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test]
        public void SetBaseTypeChokesOnNullBaseType()
        {
            var e = Assert.Throws<ArgumentNullException>(() => Factory.SetBaseType(null, ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        public class NotNotifyPropertyChanged
        {
            public void OnPropertyChanged(string name) { }
        }

    }
}
