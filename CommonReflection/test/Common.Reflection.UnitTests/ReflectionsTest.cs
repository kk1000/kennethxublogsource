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
using NUnit.Framework;

namespace Common.Reflection.UnitTests
{
    /// <summary>
    /// Test cases for <see cref="Reflections"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture(typeof(int))]
    [TestFixture(typeof(string))] 
    public class ReflectionsTest<T>
    {
        [Test] public void GetStaticInvoker_ReturnsNull_WhenParameterMisMatch()
        {
            Assert.IsNull(typeof (Base).GetStaticInvoker<Action<string, ReflectionsTest<T>>>("PublicStatic"));
        }

        [Test] public void GetStaticInvokerOrFail_Chokes_WhenParameterMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof (Base).GetStaticInvokerOrFail<Action<string, ReflectionsTest<T>>>("PublicStatic"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicStatic", e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
            StringAssert.Contains(typeof(ReflectionsTest<T>).ToString(), e.Message);
        }

        [Test] public void GetStaticInvoker_ReturnsNull_WhenReturnTypeMisMatch()
        {
            Assert.IsNull(typeof (Base).GetStaticInvoker<Func<T, double>>("PublicStatic"));
        }

        [Test] public void GetStaticInvoker_Chokes_WhenReturnTypeMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetStaticInvokerOrFail<Func<T, double>>("PublicStatic"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicStatic", e.Message);
            StringAssert.Contains(typeof(T).ToString(), e.Message);
            StringAssert.Contains(typeof(double).ToString(), e.Message);
        }

        [Test] public void GetInstanceInvoker_ReturnsNull_WhenParameterMisMatch()
        {
            Assert.IsNull(typeof(Base).GetInstanceInvoker<Action<Base, string, ReflectionsTest<T>>>("PublicInstance"));
            Assert.IsNull(new Base().GetInstanceInvoker<Action<string, ReflectionsTest<T>>>("PublicInstance"));
        }

        [Test] public void GetInstanceInvokerOrFail_Chokes_WhenParameterMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetInstanceInvokerOrFail<Action<Base, string, ReflectionsTest<T>>>("PublicInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicInstance", e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
            StringAssert.Contains(typeof(ReflectionsTest<T>).ToString(), e.Message);

            e = Assert.Throws<NoMatchException>(
                () => new Base().GetInstanceInvokerOrFail<Action<string, ReflectionsTest<T>>>("PublicInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicInstance", e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
            StringAssert.Contains(typeof(ReflectionsTest<T>).ToString(), e.Message);
        }

        [Test] public void GetInstanceInvoker_ReturnsNull_WhenReturnTypeMisMatch()
        {
            Assert.IsNull(typeof(Base).GetInstanceInvoker<Func<Base, double>>("PublicInstance"));
            Assert.IsNull(new Base().GetInstanceInvoker<Func<double>>("PublicInstance"));
        }

        [Test] public void GetInstanceInvokerOrFail_Chokes_WhenReturnTypeMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetInstanceInvokerOrFail<Func<Base, double>>("PublicInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicInstance", e.Message);
            StringAssert.Contains(typeof(T).ToString(), e.Message);
            StringAssert.Contains(typeof(double).ToString(), e.Message);

            e = Assert.Throws<NoMatchException>(
                () => new Base().GetInstanceInvokerOrFail<Func<double>>("PublicInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicInstance", e.Message);
            StringAssert.Contains(typeof(T).ToString(), e.Message);
            StringAssert.Contains(typeof(double).ToString(), e.Message);
        }

        [Test] public void GetNonVirtualInvoker_ReturnsNull_WhenParameterMisMatch()
        {
            Assert.IsNull(typeof(Base).GetNonVirtualInvoker<Action<Base, string, ReflectionsTest<T>>>("PublicVirtualInstance"));
            Assert.IsNull(new Base().GetNonVirtualInvoker<Action<string, ReflectionsTest<T>>>(typeof(Base), "PublicVirtualInstance"));
        }

        [Test] public void GetNonVirtualInvokerOrFail_Chokes_WhenParameterMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetNonVirtualInvokerOrFail<Action<Base, string, ReflectionsTest<T>>>("PublicVirtualInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicVirtualInstance", e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
            StringAssert.Contains(typeof(ReflectionsTest<T>).ToString(), e.Message);

            e = Assert.Throws<NoMatchException>(
                () => new Base().GetNonVirtualInvokerOrFail<Action<Base, string, ReflectionsTest<T>>>(typeof(Base), "PublicVirtualInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicVirtualInstance", e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
            StringAssert.Contains(typeof(ReflectionsTest<T>).ToString(), e.Message);
        }

        [Test] public void GetNonVirtualInvoker_ReturnsNull_WhenReturnTypeMisMatch()
        {
            Assert.IsNull(typeof(Base).GetNonVirtualInvoker<Func<Base, T, double>>("PublicVirtualInstance"));
            Assert.IsNull(new Base().GetNonVirtualInvoker<Func<T, double>>(typeof(Base), "PublicVirtualInstance"));
        }

        [Test] public void GetNonVirtualInvokerOrFail_Chokes_WhenReturnTypeMisMatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetNonVirtualInvokerOrFail<Func<Base, T, double>>("PublicVirtualInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicVirtualInstance", e.Message);
            StringAssert.Contains(typeof(T).ToString(), e.Message);
            StringAssert.Contains(typeof(double).ToString(), e.Message);

            e = Assert.Throws<NoMatchException>(
                () => new Base().GetNonVirtualInvokerOrFail<Func<T, double>>(typeof(Base), "PublicVirtualInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains("PublicVirtualInstance", e.Message);
            StringAssert.Contains(typeof(T).ToString(), e.Message);
            StringAssert.Contains(typeof(double).ToString(), e.Message);
        }

        [Test] public void GetStaticInvoker_Chokes_OnNonDelegateGenericParameter()
        {
            var e = Assert.Throws<InvalidOperationException>(
                () => typeof (Base).GetStaticInvoker<ReflectionsTest<T>>("PublicStatic"));
            StringAssert.Contains("Delegate type", e.Message);
            StringAssert.Contains(GetType().FullName, e.Message);
            Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetStaticInvokerOrFail<ReflectionsTest<T>>("PublicStatic"));

        }

        [Test] public void GetInstanceInvoker_Chokes_OnNonDelegateGenericParameter()
        {
            Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetInstanceInvoker<ReflectionsTest<T>>("PublicInstance"));
            Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetInstanceInvokerOrFail<ReflectionsTest<T>>("PublicInstance"));

            Assert.Throws<InvalidOperationException>(
                () => new Base().GetInstanceInvoker<ReflectionsTest<T>>("PublicInstance"));
            Assert.Throws<InvalidOperationException>(
                () => new Base().GetInstanceInvokerOrFail<ReflectionsTest<T>>("PublicInstance"));

        }

        [Test] public void GetNonVirtualInvoker_Chokes_OnNonDelegateGenericParameter()
        {
            Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetNonVirtualInvoker<ReflectionsTest<T>>("PublicVirtualInstance"));
            Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetNonVirtualInvokerOrFail<ReflectionsTest<T>>("PublicVirtualInstance"));

            Assert.Throws<InvalidOperationException>(
                () => new Base().GetNonVirtualInvoker<ReflectionsTest<T>>(typeof(Base), "PublicVirtualInstance"));
            Assert.Throws<InvalidOperationException>(
                () => new Base().GetNonVirtualInvokerOrFail<ReflectionsTest<T>>(typeof(Base), "PublicVirtualInstance"));
        }

        [Test] public void GetInstanceInvokerByType_Chokes_OnNoParameterDelegate()
        {
            var e = Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetInstanceInvoker<Action>("PublicInstance"));
            StringAssert.Contains(typeof(Action).ToString(), e.Message);
            e = Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetInstanceInvokerOrFail<Action>("PublicInstance"));
            StringAssert.Contains(typeof(Action).ToString(), e.Message);
        }

        [Test] public void GetNonVirtualInvokerByType_Chokes_OnNoParameterDelegate()
        {
            var e = Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetNonVirtualInvoker<Action>("PublicVirtualInstance"));
            StringAssert.Contains(typeof(Action).ToString(), e.Message);
            e = Assert.Throws<InvalidOperationException>(
                () => typeof(Base).GetNonVirtualInvokerOrFail<Action>("PublicVirtualInstance"));
            StringAssert.Contains(typeof(Action).ToString(), e.Message);
        }

        [Test] public void GetInstanceInvokerByType_ReturnsNull_WhenFirstParameterMismatch()
        {
            Assert.IsNull(typeof(Base).GetInstanceInvoker<Func<string, T, string, int, T>>("PublicInstance"));
        }

        [Test] public void GetInstanceInvokerOrFailByType_Chokes_WhenFirstParameterMismatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetInstanceInvokerOrFail<Func<string, T, string, int, T>>("PublicInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
        }

        [Test] public void GetNonVirtualInvokerByType_ReturnsNull_WhenFirstParameterMismatch()
        {
            Assert.IsNull(typeof(Base).GetNonVirtualInvoker<Func<string, object, T, object>>("PublicVirtualInstance"));
        }

        [Test] public void GetNonVirtualInvokerOrFailByType_Chokes_WhenFirstParameterMismatch()
        {
            var e = Assert.Throws<NoMatchException>(
                () => typeof(Base).GetNonVirtualInvokerOrFail<Func<string, object, T, object>>("PublicVirtualInstance"));
            StringAssert.Contains(typeof(Base).ToString(), e.Message);
            StringAssert.Contains(typeof(string).ToString(), e.Message);
        }

        [Test] public void GetStaticInvoker_InvokesPublicMethod()
        {
            T value = (T) Convert.ChangeType(1212456, typeof(T));
            var d = typeof (Base).GetStaticInvoker<Action<T>>("PublicStatic");
            Base.PublicStatic(value);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            d(value);
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetStaticInvokerOrFail_InvokesPrivateMethod()
        {
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = typeof(Base).GetStaticInvokerOrFail<Action<T>>("PrivateStatic");
            string expected = string.Format("PrivateStatic({0})", value);
            Base.JustCalled = null;
            d(value);
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetInstanceInvokerByType_InvokesSubPublicVirtualMethod()
        {
            Base b = new Sub();
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = typeof(Base).GetInstanceInvoker<Action<Base, T>>("PublicVirtualInstance");
            b.PublicVirtualInstance(value);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            d(b, value);
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetInstanceInvokerOrFailByType_InvokesBaseInternalMethod()
        {
            Base b = new Sub();
            T result = (T)Convert.ChangeType(384745, typeof(T));
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = typeof(Base).GetInstanceInvokerOrFail<Func<Base, T, string, int, T>>("InternalInstance");
            b.InternalInstance(value, "b", 88);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            Base.WillReturnT = result;
            Assert.AreEqual(result, d(b, value, "b", 88));
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetInstanceInvokerByInstance_InvokesSubPublicMethod()
        {
            Base b = new Sub();
            T result = (T)Convert.ChangeType(384745, typeof(T));
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = b.GetInstanceInvoker<Func<T>>("PublicInstance");
            (new Sub()).PublicInstance();
            string expected = Sub.JustCalled;
            Sub.JustCalled = null;
            Sub.WillSubReturnT = result;
            Assert.AreEqual(result, d());
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetInstanceInvokerOrFailByInstance_InvokesSubInternalVirtualMethod()
        {
            Base b = new Sub();
            object result = "result";
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = b.GetInstanceInvokerOrFail<Func<object, T, object>>("InternalProtectedVirtualInstance");
            b.InternalProtectedVirtualInstance("b", value);
            string expected = Sub.JustCalled;
            Sub.JustCalled = null;
            Sub.WillSubReturnObject = result;
            Assert.AreEqual(result, d("b", value));
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetNonVirtualInvokerByType_InvokesBasePublicVirtualMethod()
        {
            Base b = new Sub();
            object result = "result";
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = typeof(Base).GetNonVirtualInvoker<Func<Base, object, T, object>>("PublicVirtualInstance");
            new Base().PublicVirtualInstance("object", value);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            Base.WillReturnObject = result;
            Assert.AreEqual(result, d(b, "object", value));
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetNonVirtualInvokerOrFailByType_InvokesBaseInternalMethod()
        {
            Base b = new Sub();
            T result = (T)Convert.ChangeType(384745, typeof(T));
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = typeof(Base).GetNonVirtualInvokerOrFail<Func<Base, T, string, int, T>>("InternalInstance");
            b.InternalInstance(value, "b", 88);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            Base.WillReturnT = result;
            Assert.AreEqual(result, d(b, value, "b", 88));
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetNonVirtualInvokerByInstance_InvokesBasePublicMethod()
        {
            Base b = new Sub();
            T result = (T)Convert.ChangeType(384745, typeof(T));
            var d = b.GetNonVirtualInvoker<Func<T>>(typeof(Base), "PublicInstance");
            b.PublicInstance();
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            Base.WillReturnT = result;
            Assert.AreEqual(result, d());
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetNonVirtualInvokerOrFailByInstance_InvokesBaseInternalVirtualMethod()
        {
            Base b = new Sub();
            object result = "result";
            T value = (T)Convert.ChangeType(1212456, typeof(T));
            var d = b.GetNonVirtualInvokerOrFail<Func<object, T, object>>(typeof(Base), "InternalProtectedVirtualInstance");
            new Base().InternalProtectedVirtualInstance("b", value);
            string expected = Base.JustCalled;
            Base.JustCalled = null;
            Base.WillReturnObject = result;
            Assert.AreEqual(result, d("b", value));
            Assert.AreEqual(expected, Base.JustCalled);
        }

        [Test] public void GetInstanceInvokerByType_InvokesSub_OnLoseParameterMatch()
        {
            var d = typeof(Base).GetInstanceInvokerOrFail<LostParameterByTypeDelegate>("PublicVirtualInstance");
            Sub s1 = new Sub();
            Sub s2 = new Sub();
            int intout;
            s1.PublicVirtualInstance(3445, "objectb", s2, out intout);
            string expected = Sub.JustCalled;
            int expectedOut = intout;
            Sub.JustCalled = null;
            var result = d(s1, 3445, "objectb", s2, out intout);
            Assert.AreSame(s1, result);
            Assert.AreEqual(expected, Sub.JustCalled);
            Assert.AreEqual(expectedOut, intout);
        }

        [Test] public void GetInstanceInvokerByInstance_InvokesSub_OnLoseParameterMatch()
        {
            Sub s1 = new Sub();
            Sub s2 = new Sub();
            var d = s1.GetInstanceInvokerOrFail<LostParameterByInstanceDelegate>("PublicVirtualInstance");
            int intout;
            s1.PublicVirtualInstance(3445, "objectb", s2, out intout);
            string expected = Sub.JustCalled;
            int expectedOut = intout;
            Sub.JustCalled = null;
            var result = d(3445, "objectb", s2, out intout);
            Assert.AreSame(s1, result);
            Assert.AreEqual(expected, Sub.JustCalled);
            Assert.AreEqual(expectedOut, intout);
        }

        [Test] public void GetNonVirtualInvokerByType_InvokesSub_OnLoseParameterMatch()
        {
            var d = typeof(Base).GetNonVirtualInvokerOrFail<LostParameterByTypeDelegate>("PublicVirtualInstance");
            Sub s1 = new Sub();
            Sub s2 = new Sub();
            int intout;
            new Base().PublicVirtualInstance(3445, "objectb", s2, out intout);
            string expected = Base.JustCalled;
            int expectedOut = intout;
            Base.JustCalled = null;
            var result = d(s1, 3445, "objectb", s2, out intout);
            Assert.AreSame(s1, result);
            Assert.AreEqual(expected, Base.JustCalled);
            Assert.AreEqual(expectedOut, intout);
        }

        [Test] public void GetNonVirtualInvokerByInstance_InvokesSub_OnLoseParameterMatch()
        {
            Sub s1 = new Sub();
            Sub s2 = new Sub();
            var d = s1.GetNonVirtualInvokerOrFail<LostParameterByInstanceDelegate>(typeof(Base), "PublicVirtualInstance");
            int intout;
            new Base().PublicVirtualInstance(3445, "objectb", s2, out intout);
            string expected = Sub.JustCalled;
            int expectedOut = intout;
            Sub.JustCalled = null;
            var result = d(3445, "objectb", s2, out intout);
            Assert.AreSame(s1, result);
            Assert.AreEqual(expected, Sub.JustCalled);
            Assert.AreEqual(expectedOut, intout);
        }

        [Test] public void GetInvoker_FiltersMethod()
        {
            Base b = new Base();

            var dProtected = Reflections.GetInvoker<Func<T>>(
                b, b.GetType(), "ProtectedVirtualInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, null);
            Assert.NotNull(dProtected);

            var dPrivate = Reflections.GetInvoker<Action<T>>(
                b, b.GetType(), "PrivateInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, null);
            Assert.NotNull(dPrivate);

            dProtected = Reflections.GetInvoker<Func<T>>(
                b, b.GetType(), "ProtectedVirtualInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, m=>m.IsFamily);
            Assert.NotNull(dProtected);

            dPrivate = Reflections.GetInvoker<Action<T>>(
                b, b.GetType(), "PrivateInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, m => m.IsFamily);
            Assert.Null(dPrivate);
        }

        [Test] public void GetInvokerOrFail_FiltersMethod()
        {
            Base b = new Base();
            var message = "protected methods only";
            var dProtected = Reflections.GetInvokerOrFail<Func<T>>(
                b, b.GetType(), "ProtectedVirtualInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, null, null);
            Assert.NotNull(dProtected);

            var dPrivate = Reflections.GetInvokerOrFail<Action<T>>(
                b, b.GetType(), "PrivateInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, null, null);
            Assert.NotNull(dPrivate);

            dProtected = Reflections.GetInvokerOrFail<Func<T>>(
                b, b.GetType(), "ProtectedVirtualInstance",
                BindingFlags.Instance | BindingFlags.NonPublic, m=>m.IsFamily, message);
            Assert.NotNull(dProtected);

            var e = Assert.Throws<NoMatchException>(
                delegate
                    {
                        Reflections.GetInvokerOrFail<Action<T>>(
                            b, b.GetType(), "PrivateInstance",
                            BindingFlags.Instance | BindingFlags.NonPublic, m => m.IsFamily, message);
                    });
            StringAssert.Contains(message, e.Message);
        }

        private delegate IDisposable LostParameterByTypeDelegate(Sub o, int a, string b, Sub c, out int d);
        private delegate IDisposable LostParameterByInstanceDelegate(int a, string b, Sub c, out int d);

        internal class Base : IDisposable
        {
            public static string JustCalled { get; set; }
            public static T WillReturnT { get; set; }
            public static object WillReturnObject { get; set; }

            #region Public Static
            public static void PublicStatic(T a)
            {
                JustCalled = string.Format("PublicStatic({0})", a);
            }

            public static T PublicStatic()
            {
                JustCalled = string.Format("PublicStatic()");
                return WillReturnT;
            }

            public static T PublicStatic(T a, string b, int c)
            {
                JustCalled = string.Format("PublicStatic({0}, {1}, {2})", a, b, c);
                return WillReturnT;
            }

            public static object PublicStatic(object a)
            {
                JustCalled = string.Format("PublicStatic({0}", a);
                return WillReturnObject;
            }
            #endregion

            #region Non Public Static
            private static void PrivateStatic(T a)
            {
                JustCalled = string.Format("PrivateStatic({0})", a);
            }

            protected static T ProtectedStatic()
            {
                JustCalled = string.Format("ProtectedStatic()");
                return WillReturnT;
            }

            internal static T InternalStatic(T a, string b, int c)
            {
                JustCalled = string.Format("InternalStatic({0}, {1}, {2})", a, b, c);
                return WillReturnT;
            }

            internal protected static object InternalProtectedStatic(object a, T b)
            {
                JustCalled = string.Format("InternalProtectedStatic({0}, {1}", a, b);
                return WillReturnObject;
            }
            #endregion

            #region Public Instance

            public virtual Base PublicVirtualInstance(int a, object b, Base c, out int d)
            {
                JustCalled = string.Format("PublicVirtualInstance({0}, {1}, {2})", a, b, c);
                d = a + 10;
                return this;
            }
            public virtual void PublicVirtualInstance(T a)
            {
                JustCalled = string.Format("PublicVirtualInstance({0})", a);
            }

            public T PublicInstance()
            {
                JustCalled = string.Format("PublicInstance()");
                return WillReturnT;
            }

            public T PublicInstance(T a, string b, int c)
            {
                JustCalled = string.Format("PublicInstance({0}, {1}, {2})", a, b, c);
                return WillReturnT;
            }

            public virtual object PublicVirtualInstance(object a, T b)
            {
                JustCalled = string.Format("PublicVirtualInstance({0}, {1}", a, b);
                return WillReturnObject;
            }
            #endregion

            #region Non Public Instance
            private void PrivateInstance(T a)
            {
                JustCalled = string.Format("PrivateInstance({0})", a);
            }

            protected virtual T ProtectedVirtualInstance()
            {
                JustCalled = string.Format("ProtectedVirtualInstance()");
                return WillReturnT;
            }

            internal T InternalInstance(T a, string b, int c)
            {
                JustCalled = string.Format("InternalInstance({0}, {1}, {2})", a, b, c);
                return WillReturnT;
            }

            internal protected virtual object InternalProtectedVirtualInstance(object a, T b)
            {
                JustCalled = string.Format("InternalProtectedVirtualInstance({0}, {1}", a, b);
                return WillReturnObject;
            }
            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion
        }

        internal class Sub : Base
        {
            public static T WillSubReturnT { get; set; }
            public static object WillSubReturnObject { get; set; }

            public override Base PublicVirtualInstance(int a, object b, Base c, out int d)
            {
                JustCalled = string.Format("Sub.PublicVirtualInstance({0}, {1}, {2})", a, b, c);
                d = a + 100;
                return this;
            }

            public override object PublicVirtualInstance(object a, T b)
            {
                JustCalled = string.Format("Sub.PublicVirtualInstance({0}, {1}", a, b);
                return WillSubReturnObject;
            }

            public override void PublicVirtualInstance(T a)
            {
                JustCalled = string.Format("Sub.PublicVirtualInstance({0})", a);
            }

            protected internal override object InternalProtectedVirtualInstance(object a, T b)
            {
                JustCalled = string.Format("Sub.InternalProtectedVirtualInstance({0}, {1}", a, b);
                return WillSubReturnObject;
            }

            protected override T ProtectedVirtualInstance()
            {
                JustCalled = string.Format("Sub.ProtectedVirtualInstance()");
                return WillSubReturnT;
            }

            new public T PublicInstance()
            {
                JustCalled = string.Format("Sub.PublicInstance()");
                return WillSubReturnT;
            }


            new internal T InternalInstance(T a, string b, int c)
            {
                JustCalled = string.Format("Sub.InternalInstance({0}, {1}, {2})", a, b, c);
                return WillSubReturnT;
            }
        }
    }

}
