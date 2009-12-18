using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CodeSharp.Proxy.NPC;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Proxy
{
    /// <summary>
    /// Test cases for <see cref="NotifyPropertyChangeFactory"/>
    /// </summary>
    [TestFixture] public class NotifyPropertyChangeFactoryTest
    {
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

        [Test] public void SetBaseTypeChokesOnValueType() //TODO: move to somewhere else.
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(ValueType), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test] public void SetBaseTypeChokesOnTypeDoesNotImplementINotifyPropertyChanged() //TODO: move to somewhere else.
        {
            var e = Assert.Throws<ArgumentException>(() => Factory.SetBaseType(typeof(NotNotifyPropertyChanged), ""));
            Assert.That(e.ParamName, Is.EqualTo("baseType"));
        }

        [Test] public void SetBaseTypeChokesOnNullBaseType() //TODO: move to somewhere else.
        {
            var e = Assert.Throws<ArgumentNullException>(() => Factory.SetBaseType(null, ""));
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
            Factory.Reset();
            NotifyPropertyChangeFactory.SetBaseType<GoodBaseDifferentRaiser>("FirePropertyChanged");
            VerifyProxyIsBaseType<GoodBaseDifferentRaiser>();
            Factory.Reset();
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
            Factory.Reset();
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
            Factory.Reset();
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

        public class NotNotifyPropertyChanged
        {
            public void OnPropertyChanged(string name) { }
        }

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
        public interface IBar
        {
            IFoo FooProperty { get; set; }
            IKoo KooProperty { get; set; }
        }

        public class MarkingAttribute : Attribute
        {
            public Type BaseType { get; set; }
            public string OnPropertyChangedMethodName { get; set; }
        }

        public class SimpleMarkerAttribute : Attribute
        {
        }

    }
}
