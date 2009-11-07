using System;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CodeSharp.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp
{
    public interface ISimpleMethod
    {
        string SimpleMethod(int i);
    }

    [TestFixture]
    public class SimpleMethodWrapperTest
    {
        private readonly Type _interface = typeof(ISimpleMethod);
        private ModuleBuilder _moduleBuilder;
        private ISimpleMethod _mock;
        private Emitter _emmiter;
        const string _moduleName = "SimpleMethodWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _methodName = "SimpleMethod";
        private const string _wrappedFieldName = "_wrapped";
        int i = 345;
        string s = "9438jfd";

        [TestFixtureSetUp] public void TestFixtureSetUp()
        {
            _moduleBuilder = EmitUtils.CreateDynamicModule(_moduleName);
            _emmiter = new Emitter(_moduleBuilder);
        }

        [TestFixtureTearDown] public void TestFixtureTearDown()
        {
            EmitUtils.SaveAssembly(_moduleName);
        }

        [SetUp] public void SetUp()
        {
            _mock = MockRepository.GenerateStub<ISimpleMethod>();
            _mock.Stub(x => x.SimpleMethod(i)).Return(s);
        }

        [Test] public void InvokeChokesOnNonExistentMethod()
        {
            IClass c = _emmiter.Class("Nonexistent").In(_namespace);
            {
                var f = c.Field(_interface, _wrappedFieldName);

                var m = c.Method(typeof(string), _methodName, _emmiter.Arg<int>("i")).Public;
                using (var code = m.Code())
                {
                    Assert.Throws<ArgumentException>(()=> f.Invoke("NonExistentMethod", m.Arg[0]));
                }
            }
        }

        [Test] public void NonImplementingWrapper()
        {
            IClass c = _emmiter.Class("NonImplemening").In(_namespace).Public;
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = t.GetConstructor(new[] {_interface}).Invoke(new object[] {_mock});
            var simpleMethod = t.GetMethod(_methodName, new[]{typeof (int)});
            Assert.That(simpleMethod.Invoke(sut, new object[]{i}), Is.EqualTo(s));
            _mock.AssertWasCalled(x=>x.SimpleMethod(i));
        }

        [Test] public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c);
            }
            var t = _emmiter.Generate(c);
            var sut = (ISimpleMethod)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            Assert.That(sut.SimpleMethod(i), Is.EqualTo(s));
            _mock.AssertWasCalled(x => x.SimpleMethod(i));
        }

        private void CreateClassMembers(IClass c)
        {
            var wrapped = c.Field(_interface, _wrappedFieldName);

            var ctor = c.Constructor(_emmiter.Arg<ISimpleMethod>("wrapped")).Public;
            using (var code = ctor.Code())
            {
                code.Assign(wrapped, ctor.Arg[0]);
                code.Return();
            }

            var simpleMethod = c.Method(typeof(string), _methodName, _emmiter.Arg<int>("i")).Public;
            using (var code = simpleMethod.Code())
            {
                var result = wrapped.Invoke(_methodName, simpleMethod.Arg[0]);
                code.Return(result);
            }
        }
    }
}
