using System;
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

    [BvoAttribute]
    public interface IValueObject
    {
        string SimpleProperty { get; set; }
        int IntProperty { get; set; }
        int LongProperty { get; set; }
        object ObjectProperty { get; set; }
        IValueComponent ComponentProperty { get; set; }
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
            NotifyPropertyChangedProxyFactory.SetDeepProxyAttribute<BvoAttribute>();
            NotifyPropertyChangedProxyFactory.SetBaseClass<ChangeTrackerBase>("FirePropertyChanged");
        }

        [TestFixtureTearDown] public void TestFixtureTearDown()
        {
            NotifyPropertyChangedProxyFactory.SaveAssembly();
        }

        [Test] public void CanCreateProxy()
        {
            NewValueObject();
        }

        protected override IValueObject NewValueObject()
        {
            var mock = MockRepository.GenerateStub<IValueObject>();
            return NotifyPropertyChangedProxyFactory.NewProxy(mock);
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
}
