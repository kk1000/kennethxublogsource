#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

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

    /// <author>Kenneth Xu</author>
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
