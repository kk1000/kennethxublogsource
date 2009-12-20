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
using System.ComponentModel;
using CodeSharp.Proxy.NPC;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Proxy
{
    /// <summary>
    /// Test cases for <see cref="NotifyPropertyChangeFactory"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class NotifyPropertyChangeFactoryTest
    {
        [SetUp] public void ResetFactory()
        {
            Factory.Reset(false);
        }

        [Test] public void SetBaseTypeChokesOnInterfaceType()
        {
            var e = Assert.Throws<ArgumentException>(NotifyPropertyChangeFactory.SetBaseType<INotifyPropertyChanged>);
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test] public void SetBaseTypeChokesOnNonPublicType()
        {
            var e = Assert.Throws<ArgumentException>(NotifyPropertyChangeFactory.SetBaseType<NonPublicClass>);
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test] public void SetBaseTypeChokesOnSealedClass()
        {
            var e = Assert.Throws<ArgumentException>(NotifyPropertyChangeFactory.SetBaseType<SealedClass>);
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test] public void SetBaseTypeChokesOnNullOnPropertyChangedMethod()
        {
            var e = Assert.Throws<ArgumentNullException>(() => NotifyPropertyChangeFactory.SetBaseType<GoodBase>(null));
            Assert.That(e.ParamName, Is.EqualTo("onPropertyChangedMethod"));
        }

        [Test] public void SetBaseTypeChokesONonExistentOnPropertyChangedMethod()
        {
            var e = Assert.Throws<ArgumentException>(NotifyPropertyChangeFactory.SetBaseType<NoOnPropertyChangedMethod>);
            Assert.That(e.Message, Is.StringContaining(NotifyPropertyChangeFactory.DefaultOnPropertyChangedMethodName));
            const string expected = "Anything";
            e = Assert.Throws<ArgumentException>(() => NotifyPropertyChangeFactory.SetBaseType<GoodBase>(expected));
            Assert.That(e.Message, Is.StringContaining(expected));
        }

        [Test] public void SetBaseTypeChokesWhenFactoryAlreadyInitialized()
        {
            NotifyPropertyChangeFactory.GetProxy(MockRepository.GenerateStub<IFoo>());
            Assert.Throws<InvalidOperationException>(NotifyPropertyChangeFactory.SetBaseType<GoodBase>);                        
        }

        [Test] public void SetBaseTypeChangesTheBaseTypeOfGeneratedProxy()
        {
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetBaseType<GoodBaseDifferentRaiser>("FirePropertyChanged");
            VerifyProxyIsBaseType<GoodBaseDifferentRaiser>();
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetBaseType<GoodBase>();
            VerifyProxyIsBaseType<GoodBase>();
        }

        private static void VerifyProxyIsBaseType<T>()
            where T : INotifyPropertyChanged
        {
            IFoo proxy = NotifyPropertyChangeFactory.GetProxy(MockRepository.GenerateStub<IFoo>());
            Assert.That(proxy, Is.InstanceOf<T>());
            var b = (T)proxy;
            var handler = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            b.PropertyChanged += handler;
            proxy.IntPropery = 100;
            handler.AssertWasCalled(x => x(Arg<object>.Is.Same(proxy), 
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "IntPropery")));
        }

        [Test] public void SetMarkingAttributeChangesMarkingAttribute()
        {
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetMarkingAttribute<SimpleMarkerAttribute>();
            var foo = MockRepository.GenerateStub<IFoo>();
            var koo = MockRepository.GenerateStub<IKoo>();
            var bar = MockRepository.GenerateStub<IBar>();
            var barProxy = NotifyPropertyChangeFactory.GetProxy(bar);
            barProxy.FooProperty = foo;
            barProxy.KooProperty = koo;
            Assert.That(barProxy, Is.InstanceOf<NotifyPropertyChangeBase>());
            Assert.That(barProxy.FooProperty, Is.SameAs(foo));
            Assert.That(barProxy.KooProperty, Is.InstanceOf<NotifyPropertyChangeBase>());
        }

        [Test] public void SetMarkingAttributeChangesMarkingAttributeAndAllowsDifferentBaseType()
        {
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetMarkingAttribute<MarkingAttribute>(a=>a.BaseType);
            var handler = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            var foo = MockRepository.GenerateStub<IFoo>();
            var koo = MockRepository.GenerateStub<IKoo>();
            var bar = MockRepository.GenerateStub<IBar>();
            var barProxy = NotifyPropertyChangeFactory.GetProxy(bar);
            barProxy.FooProperty = foo;
            barProxy.KooProperty = koo;
            Assert.That(barProxy, Is.InstanceOf<GoodBase>());
            Assert.That(barProxy.KooProperty, Is.SameAs(koo));
            var fooProxy = barProxy.FooProperty;
            Assert.That(fooProxy, Is.InstanceOf<GoodBaseDifferentRaiser>());
            ((INotifyPropertyChanged) fooProxy).PropertyChanged += handler;
            fooProxy.IntPropery = 100;
            handler.AssertWasCalled(x => x(Arg<object>.Is.Same(fooProxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "IntPropery Raised")));
        }

        [Test] public void SetEventRaiserAttributeChokesOnNullConverter()
        {
            Assert.Throws<ArgumentNullException>( 
                ()=> NotifyPropertyChangeFactory.SetEventRaiserAttribute<RaiserAttribute>(null));
        }

        [Test] public void SetEventRaiserAttributeAllowsChangingRaiser()
        {
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetBaseType<GoodBase>();
            NotifyPropertyChangeFactory.SetEventRaiserAttribute<RaiserAttribute>(a=>a.OnPropertyChanged);
            var handler = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            var bar = MockRepository.GenerateStub<IBar>();
            var barProxy = NotifyPropertyChangeFactory.GetProxy(bar);
            Assert.That(barProxy, Is.InstanceOf<GoodBase>());
            ((INotifyPropertyChanged)barProxy).PropertyChanged += handler;
            barProxy.LongProperty = 100;
            handler.AssertWasCalled(x => x(Arg<object>.Is.Same(barProxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "LongProperty Raised")));
        }

        [Test] public void NewProxyReturnsNewInstance()
        {
            var handler1 = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            var handler2 = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            var bar = MockRepository.GenerateStub<IBar>();
            var barProxy1 = NotifyPropertyChangeFactory.NewProxy(bar);
            var barProxy2 = NotifyPropertyChangeFactory.NewProxy(bar);
            Assert.That(barProxy2, Is.Not.SameAs(barProxy1));
            ((INotifyPropertyChanged)barProxy1).PropertyChanged += handler1;
            ((INotifyPropertyChanged)barProxy2).PropertyChanged += handler2;
            barProxy1.LongProperty = 100;
            handler1.AssertWasCalled(x => x(Arg<object>.Is.Same(barProxy1),
                Arg<PropertyChangedEventArgs>.Matches(e =>e.PropertyName == "LongProperty")));
            handler2.AssertWasNotCalled(x => x(Arg<object>.Is.Same(barProxy2),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "LongProperty")));
        }

        [Test] public void NewProxyGetsNullOnNullTarget()
        {
            Assert.That(NotifyPropertyChangeFactory.NewProxy((IFoo)null), Is.Null);
        }

        [Test] public void GetProxyReturnsSameInstance()
        {
            var mock = MockRepository.GenerateStub<IBar>();
            var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
            var proxy2 = NotifyPropertyChangeFactory.GetProxy(mock);
            Assert.That(proxy2, Is.SameAs(proxy1));
        }

        [Test] public void GetProxyOnProxyReturnsItself()
        {
            {
                var mock = MockRepository.GenerateStub<IBar>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerator<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerable<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
            {
                var mock = MockRepository.GenerateStub<ICollection<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
            {
                var mock = MockRepository.GenerateStub<IList<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
            {
                var mock = MockRepository.GenerateStub<IDictionary<int,IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var proxy2 = NotifyPropertyChangeFactory.GetProxy(proxy1);
                Assert.That(proxy2, Is.SameAs(proxy1));
            }
        }

        [Test] public void GetProxyGetsNullOnNullTarget()
        {
            Assert.That(NotifyPropertyChangeFactory.GetProxy((IFoo)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetProxy((IEnumerator<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetProxy((IEnumerable<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetProxy((ICollection<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetProxy((IList<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetProxy((IDictionary<int, IFoo>)null), Is.Null);
        }

        [Test] public void GetTargetReturnsOriginalTarget()
        {
            {
                var mock = MockRepository.GenerateStub<IBar>();
                var proxy = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerator<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy1);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerable<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy1);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<ICollection<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy1);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IList<IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy1);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IDictionary<int, IBar>>();
                var proxy1 = NotifyPropertyChangeFactory.GetProxy(mock);
                var target = NotifyPropertyChangeFactory.GetTarget(proxy1);
                Assert.That(target, Is.SameAs(mock));
            }
        }

        [Test] public void GetTargetOnTargetReturnsItself()
        {
            {
                var mock = MockRepository.GenerateStub<IBar>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerator<IBar>>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IEnumerable<IBar>>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<ICollection<IBar>>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IList<IBar>>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
            {
                var mock = MockRepository.GenerateStub<IDictionary<int, IBar>>();
                var target = NotifyPropertyChangeFactory.GetTarget(mock);
                Assert.That(target, Is.SameAs(mock));
            }
        }

        [Test] public void GetTargetGetsNullOnNullProxy()
        {
            Assert.That(NotifyPropertyChangeFactory.GetTarget((IFoo)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetTarget((IEnumerator<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetTarget((IEnumerable<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetTarget((ICollection<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetTarget((IList<IFoo>)null), Is.Null);
            Assert.That(NotifyPropertyChangeFactory.GetTarget((IDictionary<int, IFoo>)null), Is.Null);
        }

        [Test] public void ProxyRaisesEventAndSetsNewValueToTarget()
        {
            var barMock = MockRepository.GenerateStub<IBar>();
            var kooMock = MockRepository.GenerateStub<IKoo>();
            var proxy = NotifyPropertyChangeFactory.GetProxy(barMock);
            var handler = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;
            const long expected = 9385;
            proxy.LongProperty = expected;
            proxy.KooProperty = kooMock;
            Assert.That(barMock.LongProperty, Is.EqualTo(expected));
            Assert.That(barMock.KooProperty, Is.SameAs(kooMock));
            handler.AssertWasCalled(x => x(Arg<object>.Is.Same(proxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "LongProperty")));
            handler.AssertWasCalled(x => x(Arg<object>.Is.Same(proxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "KooProperty")));
        }

        [Test] public void ProxyRaisesNoEventWhenSameValueWasSet()
        {
            const long expected = 9385;
            var barMock = MockRepository.GenerateStub<IBar>();
            var kooMock = MockRepository.GenerateStub<IKoo>();
            barMock.LongProperty = expected;
            barMock.KooProperty = kooMock;
            var proxy = NotifyPropertyChangeFactory.GetProxy(barMock);
            var handler = MockRepository.GenerateStub<PropertyChangedEventHandler>();
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;
            proxy.LongProperty = expected;
            proxy.KooProperty = kooMock;

            handler.AssertWasNotCalled(x => x(Arg<object>.Is.Same(proxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "LongProperty")));
            handler.AssertWasNotCalled(x => x(Arg<object>.Is.Same(proxy),
                Arg<PropertyChangedEventArgs>.Matches(e => e.PropertyName == "KooProperty")));
        }

        [Test] public void ProxyImplementsTargetProperty()
        {
            var mock = MockRepository.GenerateStub<IBigMess>();
            var proxy = NotifyPropertyChangeFactory.GetProxy(mock);
            Assert.That(proxy, Is.InstanceOf<BigMessBase>());
            Assert.That(((BigMessBase)proxy).Target, Is.SameAs(mock));
        }

        [Test] public void ProxyOverridesAbstractMemberAndUtilizeImplementationInBase()
        {
            Factory.Reset(true);
            NotifyPropertyChangeFactory.SetMarkingAttribute<MarkingAttribute>(a=>a.BaseType);
            var mock = MockRepository.GenerateStub<IBigMess>();
            var proxy = NotifyPropertyChangeFactory.GetProxy(mock);
            Assert.That(proxy, Is.InstanceOf<AbstractBigMess>());
            Assert.That(((BigMessBase)proxy).Target, Is.SameAs(mock));
        }

        public class MarkingAttribute : Attribute
        {
            public Type BaseType { get; set; }
            public string OnPropertyChangedMethodName { get; set; }
        }

        public class SimpleMarkerAttribute : Attribute {}

        public class RaiserAttribute : Attribute
        {
            public string OnPropertyChanged { get; set; }
        }

        public class GoodBaseDifferentRaiser : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void FirePropertyChanged(string name)
            {
                PropertyChangedEventHandler changed = PropertyChanged;
                if (changed != null) changed(this, new PropertyChangedEventArgs(name));
            }
            protected void RaisePropertyChanged(string name)
            {
                FirePropertyChanged(name + " Raised");
            }
        }
        
        public class GoodBase : GoodBaseDifferentRaiser
        {
            protected void OnPropertyChanged(string name)
            {
                FirePropertyChanged(name);
            }
        }

        internal class NonPublicClass : GoodBase { }

        public sealed class SealedClass : GoodBase { }

        public struct ValueType : INotifyPropertyChanged
        {
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
        }

        public class NoOnPropertyChangedMethod : INotifyPropertyChanged
        {
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
        }

        [Marking(BaseType = typeof(GoodBaseDifferentRaiser), OnPropertyChangedMethodName = "RaisePropertyChanged")]
        public interface IFoo
        {
            int IntPropery { get; set; }
        }

        [NotifyPropertyChange]
        [SimpleMarker]
        public interface IKoo
        {
            string StringProperty { get; set; }
        }

        [Marking(BaseType = typeof(GoodBase))]
        [NotifyPropertyChange]
        public interface IBar
        {
            IFoo FooProperty { get; set; }
            IKoo KooProperty { get; set; }
            [Raiser(OnPropertyChanged = "RaisePropertyChanged")]
            long LongProperty { get; set; }
        }

        [Marking(BaseType = typeof(AbstractBigMess))]
        [NotifyPropertyChange(typeof(BigMessBase))]
        public interface IBigMess
        {
            string SimpleProperty { get; set; }
            string StringProperty { get; set; }
            int IntProperty { get; set; }
            [OnPropertyChange(null)] // disable property change notification
            long LongProperty { get; set; }
            object ObjectProperty { get; set; }
            IBar ComponentProperty { get; set; }
            IList<IBar> ComponentList { get; set; }
            IDictionary<int, IBar> ComponentDictionary { get; set; }
            void MinimalMethod();
            void VoidMethod(int i);
            string ParamlessMethod();
            string SimpleMethod(int i);
            string SimpleOutRef(int i, out string s, ref long l);
            IBar DeepMethod(IBar component);
            IBar DeepOutRef(IBar a, out IBar o, ref IBigMess r);
            IBar this[IBar a, IBar o, IBigMess r] { get; set; }
        }

        public abstract class BigMessBase : NotifyPropertyChangeBase
        {
            public abstract IBigMess Target { get; }
            public abstract string SimpleProperty { get; set; }
            public string StringProperty { get; set; }
            public void MinimalMethod(string s) { }
            public void VoidMethod(int i) { }
            public abstract string ParamlessMethod();
            public string SimpleMethod(ref int i) { return i.ToString(); }
        }

        public abstract class AbstractBigMess : BigMessBase, IBigMess
        {
            public abstract int IntProperty { get; set; }
            public abstract long LongProperty { get; set; }
            public abstract object ObjectProperty { get; set; }
            public abstract IBar ComponentProperty { get; set; }
            public abstract IList<IBar> ComponentList { get; set; }
            public abstract IDictionary<int, IBar> ComponentDictionary { get; set; }
            public abstract void MinimalMethod();
            public abstract string SimpleMethod(int i);
            public abstract string SimpleOutRef(int i, out string s, ref long l);
            public abstract IBar DeepMethod(IBar component);
            public abstract IBar DeepOutRef(IBar a, out IBar o, ref IBigMess r);
            public abstract IBar this[IBar a, IBar o, IBigMess r] { get; set; }
        }
    }
}
