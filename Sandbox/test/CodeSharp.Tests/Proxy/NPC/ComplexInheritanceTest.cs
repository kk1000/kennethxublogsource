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
using Rhino.Mocks;

namespace CodeSharp.Proxy.NPC
{
    /// <summary>
    /// Class ComplexInheritanceTest
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class ComplexInheritanceTest
    {
        [SetUp]
        public void ResetFactory()
        {
            Factory.Reset(false);
        }

        [Test] public void CanCreateProxy()
        {
            var all = new Dictionary<MemberInfo, int>();
            var type = typeof(ISon); 
            var i = type.GetInterfaces();
            foreach (var parent in i)
            {
                GetMemembers(parent, all);
            }
            GetMemembers(type, all);
            ISon mock = MockRepository.GenerateStub<ISon>();
            var son = NotifyPropertyChangeFactory.GetProxy(mock);
            mock.Name = "Son";
            mock.Stub(m => m.Job).Return("NoJob");
            Assert.That(son.Name, Is.EqualTo(mock.Name));
            Assert.That(son.Age, Is.EqualTo(10));
            Assert.That(son.Job, Is.EqualTo("NoJob"));
            Assert.That(((IFather)son).Job, Is.EqualTo("NoJob"));
            Assert.That(((IMother)son).Job, Is.EqualTo("NoJob"));
            Factory.SaveAssembly();
        }

        private static void GetMemembers(Type type, IDictionary<MemberInfo, int> all)
        {
            foreach (var info in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                all[info] = 0;
            }
        }

        public interface IHuman
        {
            string Name { get; set; }
            int Age { get; }
        }

        public interface IFather : IHuman
        {
            string Job { get; }
        }

        public interface IMother : IHuman
        {
            string Job { get;  }
            object Favorite { get; }
        }

        [NotifyPropertyChange(typeof(Base))]
        public interface ISon : IFather, IMother
        {
            new string Job { get; }
            new int Age { get; }
        }

        public abstract class Base : NotifyPropertyChangeBase, IFather
        {
            public abstract string Name { get; set; }

            public int Age
            {
                get { return 10; }
            }

            public abstract string Job { get; }
        }
    }
}