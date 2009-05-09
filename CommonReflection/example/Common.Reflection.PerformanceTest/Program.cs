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
using System.Reflection;
using System.Reflection.Emit;

namespace Common.Reflection.PerformanceTest
{
    /// <summary>
    /// Compare performance between a) Direct method call. b) Reflection Invoke
    /// c) Delegate call.
    /// </summary>
    /// <author>Kenneth Xu</author>
    static class Program
    {
        const int loop = 100000000;
        const string methodName = "PerfTest";

        static void Main(string[] args)
        {
            PerformanceTest();
        }

        public static void PerformanceTest()
        {
            DirectCallPerformanceTest();

            RegularDelegatePerformanceTest();

            MethodInfoInvokePerformanceTest();

            MethodInfoDelegatePerformanceTest();

            DynamicMethodInvokePerformanceTest();

            DynamicMethodDelegatePerformanceTest();
        }

        private static void RegularDelegatePerformanceTest()
        {
            DelegatePerformanceTest("Regular Delegate", new Sub().PerfTestDelegate);
        }

        private static void MethodInfoDelegatePerformanceTest()
        {
            var methodInfoDelegate = new Sub().GetInstanceMethod<Func<int, object, int>>(methodName);
            DelegatePerformanceTest("MethodInfo Delegate", methodInfoDelegate);
        }

        private static void DynamicMethodDelegatePerformanceTest()
        {
            var dynamicMethodDelegate = new Sub().GetNonVirtualInvoker<Func<int, object, int>>(typeof(Base), methodName);
            DelegatePerformanceTest("DynamicMethod Delegate", dynamicMethodDelegate);
        }

        private static void DirectCallPerformanceTest()
        {
            Base sub = new Sub();
            object o = new object();

            DateTime start = DateTime.Now;
            for (int i = loop; i > 0; i--)
            {
                sub.PerfTest(0, o);
            }
            WriteResult("Direct Method Call", (DateTime.Now - start).TotalMilliseconds);
        }

        private static void MethodInfoInvokePerformanceTest()
        {
            Base sub = new Sub();
            MethodInfo methodInfo = sub.GetType().GetMethod(methodName);
            object o = new object();
            DateTime start = DateTime.Now;
            for (int i = loop / 1000; i > 0; i--)
            {
                methodInfo.Invoke(sub, new object[] { 1, o });
            }
            WriteResult("MethodInfo.Invoke", (DateTime.Now - start).TotalMilliseconds * 1000);
        }

        private static void DynamicMethodInvokePerformanceTest()
        {
            Base sub = new Sub();
            DynamicMethod dynamicMethod = Reflections.CreateDynamicMethod(typeof(Base).GetMethod(methodName));
            object o = new object();
            DateTime start = DateTime.Now;
            for (int i = loop / 1000; i > 0; i--)
            {
                dynamicMethod.Invoke(null, new object[] { sub, 1, o });
            }
            WriteResult("DynamicMethod.Invoke", (DateTime.Now - start).TotalMilliseconds * 1000);
        }

        private static void DelegatePerformanceTest(string testName, Func<int, object, int> callDelegate)
        {
            object o = new object();
            DateTime start = DateTime.Now;
            for (int i = loop; i > 0; i--)
            {
                callDelegate(1, o);
            }
            WriteResult(testName, (DateTime.Now - start).TotalMilliseconds);
        }

        private static void WriteResult(string callType, double milliSeconds)
        {
            var s = string.Format("{0:#,##0} invocations using {1,-22}: {2,7:#,##0}ms", loop, callType, milliSeconds);
            Console.Out.WriteLine(s);
        }

        private class Base
        {
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
            public override int PerfTest(int i, object o)
            {
                return 1;
            }
        }

    }
}
