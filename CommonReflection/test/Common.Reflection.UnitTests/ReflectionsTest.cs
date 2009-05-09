#region License

/*
 * Copyright (C) 2009 the original author or authors.
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
using System.Reflection.Emit;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Common.Reflection.UnitTests
{
    /// <summary>
    /// Test cases for <see cref="Reflections"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class ReflectionsTest
    {
        [Test] public void ChokesOnMethodMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof (Base).GetStaticMethod<Action<string, ReflectionsTest>>("PublicStatic"));
            StringAssert.Contains(typeof(Base).FullName, e.Message);
            StringAssert.Contains("PublicStatic", e.Message);
            StringAssert.Contains(typeof(string).FullName, e.Message);
            StringAssert.Contains(typeof(ReflectionsTest).FullName, e.Message);
        }

        [Test] public void ChokesOnNonDelegateGenericParameter()
        {
            var e = Assert.Throws<InvalidOperationException>(
                () => typeof (Base).GetStaticMethod<ReflectionsTest>("PublicStatic"));
            StringAssert.Contains("Delegate type", e.Message);
            StringAssert.Contains(GetType().FullName, e.Message);
        }

        [Test] public void GetMethod_PublicStatic()
        {
            var call = typeof(Base).GetStaticMethod<Action<string>>("PublicStatic");
            call("tsting");
        }

        [Test] public void GetMethod_PublicInstance_ByType()
        {
            var fake = new Base();
            var call = fake.GetType().GetInstanceMethod<Action<Base, string>>("PublicInstance");
            call(fake, "tsting");
        }

        [Test] public void GetMethod_PublicInstance()
        {
            var fake = new Base();
            var call = fake.GetInstanceMethod<Action<string>>("PublicInstance");
            call("tsting");
        }

        [Test] public void GetMethod_PublicVirtual_ByType()
        {
            var sub = new Sub();
            var call = typeof(Base).GetNonVirtualInvoker<Func<Base, string, int>>("PublicVirtual");
            call(sub, "tsting");
        }

        [Test] public void GetMethod_PublicVirtual()
        {
            var sub = new Sub();
            var call = sub.GetNonVirtualInvoker<Func<string, int>>(typeof(Base), "PublicVirtual");
            call("tsting");
        }


        private class Base
        {
            public static void PublicStatic(string a)
            {
                Console.WriteLine("public static void PublicStatic(string a)");
            }

            public static int PublicStatic(int i)
            {
                Console.WriteLine("public static int PublicStatic(int i)");
                return 0;
            }

            public void PublicInstance(string a)
            {
                Console.WriteLine("public void PublicInstance(string a)");
            }

            public virtual int PublicVirtual(string a)
            {

                Console.WriteLine("public virtual int PublicVirtual(string a)");
                return 0;
            }

            public virtual int PerfTest(int i, object o)
            {
                return 0;
            }

            public Func<int, object, int> PerfTestDelegate
            {
                get { return PerfTest; }
            }
        }

        private class Sub : Base
        {
            public override int PublicVirtual(string a)
            {
                Console.WriteLine("public override int PublicVirtual(string a)");
                return 1;
            }

            public override int PerfTest(int i, object o)
            {
                return 1;
            }
        }
    }

}
