using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;
using CodeSharp.Emit;

namespace CodeSharp
{
    public interface IReadWriteProperty
    {
        int ReadWriteProperty { get; set; }
    }

    [TestFixture]
    public class ReadWritePropertyWrapperTest
    {
        private static readonly Type _interface = typeof(IReadWriteProperty);
        private ModuleBuilder _moduleBuilder;
        private IReadWriteProperty _mock;
        private Emitter _emmiter;
        const string _moduleName = "ReadWritePropertyWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _propertyName = "ReadWriteProperty";
        private const string _wrappedFieldName = "_wrapped";
        const int _getValue = 93858;
        private const int _setValue = 491743;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _moduleBuilder = EmitUtils.CreateDynamicModule(_moduleName);
            _emmiter = new Emitter(_moduleBuilder);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EmitUtils.SaveAssembly(_moduleName);
        }

        [SetUp]
        public void SetUp()
        {
            _mock = MockRepository.GenerateMock<IReadWriteProperty>();
            _mock.Stub(x => x.ReadWriteProperty).Return(_getValue);
        }

        [Test]
        public void NonImplementingWrapper()
        {
            IClass c = _emmiter.Class("NonImplemening").In(_namespace).Public;
            {
                CreateClassMembers(c, _emmiter);
            }
            var t = _emmiter.Generate(c);
            var sut = t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            var readWriteProperty = t.GetProperty(_propertyName);
            Assert.That(readWriteProperty.GetValue(sut, null), Is.EqualTo(_getValue));
            _mock.AssertWasCalled(x => x.ReadWriteProperty);
            readWriteProperty.SetValue(sut, _setValue, null);
            _mock.AssertWasCalled(x => x.ReadWriteProperty = _setValue);
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c, _emmiter);
            }
            var t = _emmiter.Generate(c);
            var sut = (IReadWriteProperty)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            Assert.That(sut.ReadWriteProperty, Is.EqualTo(_getValue));
            sut.ReadWriteProperty = _setValue;
            _mock.AssertWasCalled(x => x.ReadWriteProperty = _setValue);
            _mock.AssertWasCalled(x => x.ReadWriteProperty);
        }

        private static void CreateClassMembers(IClass t, IGenerator g)
        {
            var wrapped = t.Field(_interface, _wrappedFieldName);

            var ctor = t.Constructor(g.Arg<IReadWriteProperty>("wrapped")).Public;
            using (var c = ctor.Code())
            {
                c.Assign(wrapped, ctor.Arg[0]);
            }

            var readWriteProperty = t.Property(typeof(int), _propertyName).Public;
            var getter = readWriteProperty.Getter();
            using (var c = getter.Code())
            {
                c.Return(wrapped.Property(_propertyName));
            }
            var setter = readWriteProperty.Setter();
            using (var c = setter.Code())
            {
                c.Assign(wrapped.Property(_propertyName), setter.Value);
            }
        }
    }
}
