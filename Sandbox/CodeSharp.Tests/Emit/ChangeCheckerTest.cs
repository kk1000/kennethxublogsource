using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeSharp.Proxy;
using NUnit.CommonFixtures;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Emit
{
    public class BvoAttribute : Attribute
    {
        private Type _notifyPropertyChangedBase;

        public Type NotifyPropertyChangedBase
        {
            get { return _notifyPropertyChangedBase; }
            set
            {
                if (value != null && !typeof(ChangeTrackerBase).IsAssignableFrom(value))
                {
                    throw new ArgumentException("Must be sub class of " + typeof(ChangeTrackerBase), "value");
                }
                _notifyPropertyChangedBase = value;
            }
        }
    }

    [Bvo(NotifyPropertyChangedBase = typeof(ValueObjectProxyBase))]
    public interface IValueObject
    {
        string SimpleProperty { get; set; }
        int IntProperty { get; set; }
        int LongProperty { get; set; }
        object ObjectProperty { get; set; }
        IValueComponent ComponentProperty { get; set; }
        IList<IValueComponent> ComponentList { get; set; }
        IDictionary<int, IValueComponent> ComponentDictionary { get; set; }
        void MinimalMethod();
        void VoidMethod(int i);
        string ParamlessMethod();
        string SimpleMethod(int i);
        string SimpleOutRef(int i, out string s, ref long l);
        IValueComponent DeepMethod(IValueComponent component);
        IValueComponent DeepOutRef(IValueComponent a, out IValueComponent o, ref IValueObject r);
        IValueComponent this[IValueComponent a, IValueComponent o, IValueObject r] { get; set;}
    }

    public abstract class ValueObjectProxyBase : ChangeTrackerBase
    {
        protected abstract IValueObject Target { get; }
        public abstract int IntProperty { get; set; }
        public virtual int LongProperty {
            get { return 0; }
            set { }
        }
    }

    [Bvo]
    public interface IValueComponent
    {
        string StringProperty { get; set; }
        object ObjectProperty { get; set; }
    }

    [TestFixture]
    public class ChangeCheckerTest : ValueObjectTestFixture<IValueObject>
    {
        public ChangeCheckerTest()
        {
            ExcludeProperties("ComponentProperty");
        }
        [TestFixtureSetUp] public void TestFixtureSetUp()
        {
            NotifyPropertyChangeFactory.SetMarkingAttribute<BvoAttribute>(b=>b.NotifyPropertyChangedBase);
            NotifyPropertyChangeFactory.SetBaseType<ChangeTrackerBase>("FirePropertyChanged");
        }

        [TestFixtureTearDown] public void TestFixtureTearDown()
        {
            NotifyPropertyChangeFactory.SaveAssembly();
        }

        [Test] public void CanCreateProxy()
        {
            var o = NewValueObject();
            var mock = NotifyPropertyChangeFactory.GetTarget(o);
            mock.ComponentProperty = MockRepository.GenerateStub<IValueComponent>();
            var x = o.ComponentProperty;
        }

        protected override IValueObject NewValueObject()
        {
            var mock = MockRepository.GenerateStub<IValueObject>();
            return NotifyPropertyChangeFactory.NewProxy(mock);
        }

        protected override System.Collections.IEnumerable TestData(PropertyInfo property)
        {
            if (property.PropertyType == typeof(IValueComponent))
            {
                return new[]
                           {
                               MockRepository.GenerateStub<IValueComponent>(),
                               null,
                               MockRepository.GenerateStub<IValueComponent>(),
                           };
            }
            return base.TestData(property);
        }
    }


    public class Foo
    {
        public void Bar()
        {
            var getProxy = new Dictionary<string, MethodInfo>();
            var getTarget = new Dictionary<string, MethodInfo>();
            var members = typeof (NotifyPropertyChangeFactory).GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in members.Where(m=>m.MemberType == MemberTypes.Method))
            {
                switch (method.Name)
                {
                    case "GetProxy":
                        getProxy[method.GetParamTypes()[0].ToString()] = method;
                        break;
                    case "GetTarget":
                        getTarget[method.GetParamTypes()[0].ToString()] = method;
                        break;
                }
            }
            Console.WriteLine(getProxy[typeof(IList<>).ToString()]);
        }

        public void GetProxy<T>(IList<T> s)
        {
        }
    }
}
