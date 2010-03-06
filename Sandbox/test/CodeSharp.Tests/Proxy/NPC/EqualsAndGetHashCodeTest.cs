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
using System.Reflection;
using NUnit.Framework;

namespace CodeSharp.Proxy.NPC
{
    /// <summary>
    /// Class EqualsAndGetHashCodeTest
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class EqualsAndGetHashCodeTest
    {
        [SetUp]
        public void ResetFactory()
        {
            Factory.Reset(false);
        }

        [Test] public void GetProxyReturnsSameInstanceForSameTarget()
        {
            IFoo target1 = new Foo();

            var proxy1 = NotifyPropertyChangeFactory.GetProxy(target1);

            target1.Name = "New Name";
            target1.Age = 100;

            var proxy2 = NotifyPropertyChangeFactory.GetProxy(target1);

            Assert.That(proxy2, Is.SameAs(proxy1));
        }

        [Test]
        public void GetProxyReturnsDifferenceInstancesForDifferentButEqualTargets()
        {
            IFoo target1 = new Foo();
            IFoo target2 = new Foo();

            var proxy1 = NotifyPropertyChangeFactory.GetProxy(target1);
            var proxy2 = NotifyPropertyChangeFactory.GetProxy(target2);

            Assert.That(target2, Is.EqualTo(target1));
            Assert.That(proxy2, Is.Not.EqualTo(proxy1));
        }

        public interface IFoo
        {
            string Name { get; set; }
            int Age { get; set;  }
        }

        public class Foo : IFoo
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public bool Equals(Foo other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name) && other.Age == Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Foo)) return false;
                return Equals((Foo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Age;
                }
            }
        }
    }
}