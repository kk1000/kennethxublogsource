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
        private readonly Type _baseType;

        public BvoAttribute()
        {
        }

        public BvoAttribute(Type baseType)
        {
            if (baseType != null && !typeof(ChangeTrackerBase).IsAssignableFrom(baseType))
            {
                throw new ArgumentException("Must be sub class of " + typeof(ChangeTrackerBase), "baseType");
            }
            _baseType = baseType;
        }

        public Type BaseType
        {
            get { return _baseType; }
        }
    }

    [BvoAttribute(typeof(ChangeTrackerBase))]
    public interface IValueObject
    {
        string SimpleProperty { get; set; }
        int IntProperty { get; set; }
        int LongProperty { get; set; }
        object ObjectProperty { get; set; }
        IValueComponent ComponentProperty { get; set; }
        IList<IValueComponent> ComponentList { get; set; }
        IDictionary<int, IValueComponent> ComponentDictionary { get; set; }
    }

    [BvoAttribute]
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
            NotifyPropertyChangedFactory.SetDeepProxyAttribute<BvoAttribute>();
            NotifyPropertyChangedFactory.SetBaseClass<ChangeTrackerBase>("FirePropertyChanged");
        }

        [TestFixtureTearDown] public void TestFixtureTearDown()
        {
            NotifyPropertyChangedFactory.SaveAssembly();
        }

        [Test] public void CanCreateProxy()
        {
            NewValueObject();
        }

        protected override IValueObject NewValueObject()
        {
            var mock = MockRepository.GenerateStub<IValueObject>();
            return NotifyPropertyChangedFactory.NewProxy(mock);
        }

        protected override System.Collections.IEnumerable TestData(System.Reflection.PropertyInfo property)
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
            var members = typeof (NotifyPropertyChangedFactory).GetMembers(BindingFlags.Static | BindingFlags.Public);
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
