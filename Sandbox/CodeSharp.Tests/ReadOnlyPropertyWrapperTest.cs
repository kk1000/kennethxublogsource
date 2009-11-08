using System;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CodeSharp.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp
{
    public interface IReadOnlyProperty
    {
        int ReadOnlyProperty { get; }
    }

    [TestFixture]
    public class ReadOnlyPropertyWrapperTest
    {
        private readonly Type _interface = typeof(IReadOnlyProperty);
        private ModuleBuilder _moduleBuilder;
        private IReadOnlyProperty _mock;
        private Emitter _emmiter;
        const string _moduleName = "ReadOnlyPropertyWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _propertyName = "ReadOnlyProperty";
        private const string _wrappedFieldName = "_wrapped";
        const int _expected = 93858;

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
            _mock = MockRepository.GenerateStub<IReadOnlyProperty>();
            _mock.Stub(x => x.ReadOnlyProperty).Return(_expected);
        }

        [Test]
        public void InvokeChokesOnNonExistentMethod()
        {
            IClass c = _emmiter.Class("Nonexistent").In(_namespace);
            {
                var f = c.Field(_interface, _wrappedFieldName);

                var p = c.Property(typeof(string), _propertyName).Public;
                var m = p.Getter();
                using (m.Code())
                {
                    var e = Assert.Throws<ArgumentException>(() => f.Property("NonExistentProperty"));
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        [Test]
        public void NonImplementingWrapper()
        {
            IClass c = _emmiter.Class("NonImplemening").In(_namespace).Public;
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            var readOnlyProperty = t.GetProperty(_propertyName);
            Assert.That(readOnlyProperty.GetValue(sut, null), Is.EqualTo(_expected));
            _mock.AssertWasCalled(x => x.ReadOnlyProperty);
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = (IReadOnlyProperty)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            Assert.That(sut.ReadOnlyProperty, Is.EqualTo(_expected));
            _mock.AssertWasCalled(x => x.ReadOnlyProperty);
        }

        private void CreateClassMembers(IClass c)
        {
            var wrapped = c.Field(_interface, _wrappedFieldName);

            var ctor = c.Constructor(_emmiter.Arg<IReadOnlyProperty>("wrapped")).Public;
            using (var code = ctor.Code())
            {
                code.Assign(wrapped, ctor.Args[0]);
            }

            var readOnlyProperty = c.Property(typeof(int), _propertyName).Public;
            var m = readOnlyProperty.Getter();
            using (var code = m.Code())
            {
                code.Return(wrapped.Property(_propertyName));
            }
        }
    }
}
