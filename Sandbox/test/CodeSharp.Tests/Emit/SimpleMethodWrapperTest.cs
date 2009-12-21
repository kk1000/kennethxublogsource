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
    public interface ISimpleMethod
    {
        string SimpleMethod(int i);
    }

    /// <author>Kenneth Xu</author>
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
                    var e = Assert.Throws<ArgumentException>(()=> f.Invoke("NonExistentMethod", m.Args[0]));
                    System.Diagnostics.Debug.WriteLine(e.Message);
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
                code.Assign(wrapped, ctor.Args[0]);
                code.Return();
            }

            var simpleMethod = c.Method(typeof(string), _methodName, _emmiter.Arg<int>("i")).Public;
            using (var code = simpleMethod.Code())
            {
                var result = wrapped.Invoke(_methodName, simpleMethod.Args[0]);
                code.Return(result);
            }
        }
    }
}
