using System;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CodeSharp.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp
{
    public interface IWriteOnlyProperty
    {
        int WriteOnlyProperty { set; }
    }

    [TestFixture]
    public class WriteOnlyPropertyWrapperTest
    {
        private readonly Type _interface = typeof(IWriteOnlyProperty);
        private ModuleBuilder _moduleBuilder;
        private IWriteOnlyProperty _mock;
        private Emitter _emmiter;
        const string _moduleName = "WriteOnlyPropertyWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _propertyName = "WriteOnlyProperty";
        private const string _wrappedFieldName = "_wrapped";
        const int _expectedValue = 93939;

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
            _mock = MockRepository.GenerateStub<IWriteOnlyProperty>();
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
            var writeOnlyProperty = t.GetProperty(_propertyName);
            writeOnlyProperty.SetValue(sut, _expectedValue, null);
            _mock.AssertWasCalled(x => x.WriteOnlyProperty = _expectedValue);
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = (IWriteOnlyProperty)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            sut.WriteOnlyProperty = _expectedValue;
            _mock.AssertWasCalled(x => x.WriteOnlyProperty = _expectedValue);
        }

        private void CreateClassMembers(IClass c)
        {
            var wrapped = c.Field(_interface, _wrappedFieldName);

            var ctor = c.Constructor(_emmiter.Arg<IWriteOnlyProperty>("wrapped")).Public;
            using (var code = ctor.Code())
            {
                code.Assign(wrapped, ctor.Arg[0]);
            }

            var writeOnlyProperty = c.Property(typeof(int), _propertyName).Public;
            var m = writeOnlyProperty.Setter();
            using (var code = m.Code())
            {
                code.Assign(wrapped.Property(_propertyName), m.Value);
            }
        }
    }
}
