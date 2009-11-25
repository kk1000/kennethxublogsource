using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Emit
{
    public interface IOutParamMethod
    {
        void OutParamMethod(out int i);
    }

    [TestFixture]
    public class OutParameterMethodWrapperTest
    {
        private readonly Type _interface = typeof(IOutParamMethod);
        private ModuleBuilder _moduleBuilder;
        private IOutParamMethod _mock;
        private IGenerator _g;
        const string _moduleName = "OutParameterMethodWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _methodName = "OutParamMethod";
        private const string _wrappedFieldName = "_wrapped";
        int _refParameter;
        const int _expectedValue = 93939;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _moduleBuilder = EmitUtils.CreateDynamicModule(_moduleName);
            _g = new Emitter(_moduleBuilder);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EmitUtils.SaveAssembly(_moduleName);
        }

        [SetUp]
        public void SetUp()
        {
            _refParameter = 345;
            _mock = MockRepository.GenerateStub<IOutParamMethod>();
            _mock.Stub(x => x.OutParamMethod(out _refParameter)).OutRef(_expectedValue);
        }

        [Test]
        public void NonImplementingWrapper()
        {
            IClass c = _g.Class("NonImplemening").In(_namespace).Public;
            {
                CreateClassMembers(c);
            }
            var t = ((Emitter)_g).Generate(c);
            var sut = t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            var simpleMethod = t.GetMethod(_methodName, new[] { typeof(int).MakeByRefType() });
            object[] parameters = new object[] { _refParameter };
            simpleMethod.Invoke(sut, parameters);
            _refParameter = (int)parameters[0];
            Assert.That(_refParameter, Is.EqualTo(_expectedValue));
            _mock.AssertWasCalled(x => x.OutParamMethod(out _refParameter));
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _g.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c);
            }
            var t = ((Emitter)_g).Generate(c);
            var sut = (IOutParamMethod)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            sut.OutParamMethod(out _refParameter);
            Assert.That(_refParameter, Is.EqualTo(_expectedValue));
            _mock.AssertWasCalled(x => x.OutParamMethod(out _refParameter));
        }

        private void CreateClassMembers(IClass c)
        {
            var wrapped = c.Field(_interface, _wrappedFieldName);

            var ctor = c.Constructor(_g.Arg<IOutParamMethod>("wrapped")).Public;
            using (var code = ctor.Code())
            {
                code.Assign(wrapped, ctor.Args[0]);
            }

            var simpleMethod = c.Method(_methodName, _g.ArgOut<int>("i")).Public;
            using (var code = simpleMethod.Code())
            {
                var result = wrapped.Invoke(_methodName, simpleMethod.Args[0]);
                code.Return(result);
            }
        }
    }
}
