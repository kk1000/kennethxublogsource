using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Emit
{
    public interface IRefParamMethod
    {
        void RefParamMethod(ref long y);
    }

    [TestFixture]
    public class RefParameterMethodWrapperTest
    {
        private readonly Type _interface = typeof(IRefParamMethod);
        private ModuleBuilder _moduleBuilder;
        private IRefParamMethod _mock;
        private Emitter _emmiter;
        const string _moduleName = "RefParameterMethodWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _methodName = "RefParamMethod";
        private const string _wrappedFieldName = "_wrapped";
        long _refParameter;
        const long _expectedValue = 93939;

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
            _refParameter = 345;
            _mock = MockRepository.GenerateStub<IRefParamMethod>();
            _mock.Stub(x => x.RefParamMethod(ref _refParameter)).OutRef(_expectedValue);
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
            var simpleMethod = t.GetMethod(_methodName, new[] { typeof(long).MakeByRefType() });
            object[] parameters = new object[] {_refParameter};
            simpleMethod.Invoke(sut, parameters);
            _refParameter = (long) parameters[0];
            Assert.That(_refParameter, Is.EqualTo(_expectedValue));
            _mock.AssertWasCalled(x => x.RefParamMethod(ref _refParameter));
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = (IRefParamMethod)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            sut.RefParamMethod(ref _refParameter);
            Assert.That(_refParameter, Is.EqualTo(_expectedValue));
            _mock.AssertWasCalled(x => x.RefParamMethod(ref _refParameter));
        }

        private void CreateClassMembers(IClass c)
        {
            var wrapped = c.Field(_interface, _wrappedFieldName);

            var ctor = c.Constructor(_emmiter.Arg<IRefParamMethod>("wrapped")).Public;
            using (var code = ctor.Code())
            {
                code.Assign(wrapped, ctor.Args[0]);
                code.Return();
            }

            var simpleMethod = c.Method(_methodName, _emmiter.ArgRef<long>("y")).Public;
            using (var code = simpleMethod.Code())
            {
                var result = wrapped.Invoke(_methodName, simpleMethod.Args[0]);
                code.Return(result);
            }
        }
    }
}
