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
using System.Linq;
using NUnit.Framework;

namespace CodeSharp.Proxy.NPC
{
    /// <summary>
    /// Class EqualsAndGetHashCodeTest
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class CollectionProxyTest
    {
        private IBar _target;
        [SetUp]
        public void ResetFactory()
        {
            Factory.Reset(false);
            _target =
                new Bar
                {
                    FooDictionary =
                        new Dictionary<int, IFoo>
                                {
                                    {0, new Foo {Name = "No.0"}},
                                    {1, new Foo {Name = "No.1"}},
                                },
                    FooList =
                        new List<IFoo>
                                {
                                    new Foo {Name = "No.3"},
                                    new Foo {Name = "No.4"},
                                },
                    FooListDictionary =
                        new Dictionary<string, IList<IFoo>>
                                {
                                    {"A", new List<IFoo>{ new Foo{Name="No.5"}, new Foo{Name="No.6"}}},
                                    {"B", new List<IFoo>{ new Foo{Name="No.7"}, new Foo{Name="No.8"}}},
                                }
                };
        }

        [Test] public void ListPropertyHandlesProxyAsExpected()
        {
            var proxy = NotifyPropertyChangeFactory.GetProxy(_target);

            Assert.That(proxy.FooList, Is.Not.SameAs(_target.FooList));
            Assert.That(proxy.FooList[0].Name, Is.SameAs("No.3"));
            Assert.That(proxy.FooList[0], Is.InstanceOf<FooProxy>());
            Assert.That(proxy.FooList.First().Name, Is.SameAs("No.3"));
            Assert.That(proxy.FooList.First(), Is.InstanceOf<FooProxy>());
        }

        [Test]
        public void DictionaryPropertyHandlesProxyAsExpected()
        {
            var proxy = NotifyPropertyChangeFactory.GetProxy(_target);

            var proxyDictionary = proxy.FooDictionary;
            Assert.That(proxyDictionary, Is.Not.SameAs(_target.FooDictionary));
            // by indexer
            Assert.That(proxyDictionary[0].Name, Is.SameAs("No.0"));
            Assert.That(proxyDictionary[0], Is.InstanceOf<FooProxy>());
            // by enuemrator
            Assert.That(proxyDictionary.First().Value.Name, Is.SameAs("No.0"));
            Assert.That(proxyDictionary.First().Value, Is.InstanceOf<FooProxy>());
            // by enumerator of values collection
            Assert.That(proxyDictionary.Values.First().Name, Is.SameAs("No.0"));
            Assert.That(proxyDictionary.Values.First(), Is.InstanceOf<FooProxy>());

            var toAdd = new Foo { Name = "No.-1", Age = -1 };

            proxyDictionary.Add(toAdd.Age, toAdd);
            Assert.That(proxyDictionary[toAdd.Age].Name, Is.EqualTo(toAdd.Name));
            Assert.That(proxyDictionary[toAdd.Age], Is.InstanceOf<FooProxy>());
        }

        [Test]
        public void NestedCollectionPropertyHandlesProxyAsExpected()
        {
            var target = _target;

            var proxy = NotifyPropertyChangeFactory.GetProxy(target);

            Assert.That(proxy.FooListDictionary["A"][0].Name, Is.EqualTo("No.5"));
            Assert.That(proxy.FooListDictionary["A"][0], Is.InstanceOf<FooProxy>());
        }

        [NotifyPropertyChange]
        public interface IBar
        {
            IList<IFoo> FooList { get; set;}
            IDictionary<int, IFoo> FooDictionary { get; set; }
            IDictionary<string, IList<IFoo>> FooListDictionary { get; set; }
        }

        public class Bar : IBar
        {
            public IList<IFoo> FooList { get; set; }
            public IDictionary<int, IFoo> FooDictionary { get; set; }
            public IDictionary<string, IList<IFoo>> FooListDictionary { get; set; }
        }

        [NotifyPropertyChange(typeof(FooProxy))]
        public interface IFoo
        {
            string Name { get; set; }
            int Age { get; set;  }
        }

        public class Foo : IFoo
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class FooProxy : NotifyPropertyChangeBase
        {
            
        }
    }
}