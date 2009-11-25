using System;
using NUnit.CommonFixtures;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Emit
{
    public class BvoAttribute : Attribute{}

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
            ChangeTrackerProxyFactory.SetDeepProxyAttribute<BvoAttribute>();
        }

        [TestFixtureTearDown] public void TestFixtureTearDown()
        {
            ChangeTrackerProxyFactory.SaveAssembly();
        }

        [Test] public void CanCreateProxy()
        {
            NewValueObject();
        }

        protected override IValueObject NewValueObject()
        {
            var mock = MockRepository.GenerateStub<IValueObject>();
            return ChangeTrackerProxyFactory.NewProxy(mock);
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
