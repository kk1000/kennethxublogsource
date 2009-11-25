using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeSharp.Emit
{
    public interface IHodgepodge
    {
        int ReadWriteValueTypeProperty { get; set; }
        long ReadOnlyValueTypeProperty { get; }
        DateTime WriteOnlyValueTypeProperty { set; }

        object ReadWriteReferenceTypeProperty { get; set; }
        string ReadOnlyReferenceTypeProperty { get; }
        Action<int> WriteOnlyReferenceTypeProperty { set; }

        string this[int index] { get; set; }
        long this[string s, int i] { get; }
        object this[short s] { set; }

        void MinimalMethod();

        string ParameterlessMethod();

        void VariableArgumentMethod(params string[] any);

        void LongParameterMethod(int p1, string p2, long p3, int[] p4, object p5, Action<string> p6);

        int ValueTypeRefOutMethod(int i, out int x, ref long y);

        object ReferenceTypeRefOutMethod(string s, out string x, ref Action<int> y);
    }

    [TestFixture]
    public class HodgepodgeWrapperTest
    {
        const string _moduleName = "HodgepodgeWrapperTest.dll";
        const string _namespace = _moduleName;
        private const string _wrappedFieldName = "_wrapped";
        private static readonly Type _interface = typeof(IHodgepodge);

        private ModuleBuilder _moduleBuilder;
        private IGenerator _g;
        private IHodgepodge _mock;

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
            _mock = MockRepository.GenerateMock<IHodgepodge>();
        }

        [Test] public void SunnyDay()
        {
            var t = ((Emitter)_g).Generate(DefineWrapper());
            var sut = (IHodgepodge)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });

            //Assert.That(sut.ReadWriteProperty, Is.EqualTo(_getValue));
            //sut.ReadWriteProperty = _setValue;
            //_mock.AssertWasCalled(x => x.ReadWriteProperty = _setValue);
            //_mock.AssertWasCalled(x => x.ReadWriteProperty);
        }

        private IClass DefineWrapper()
        {
            IClass c = _g.Class("HodgepodgeWrapper").In(_namespace).Implements(_interface);
            {
                var wrapped = c.Field(_interface, _wrappedFieldName);

                var ctor = c.Constructor(_g.Arg<IHodgepodge>("wrapped")).Public;
                using (var c1 = ctor.Code())
                {
                    c1.Assign(wrapped, ctor.Args[0]);
                }

                foreach (MemberInfo info in _interface.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                {
                    switch (info.MemberType)
                    {
                        case MemberTypes.Method:
                            MethodInfo mi = (MethodInfo)info;
                            if (!mi.IsSpecialName)
                            {
                                WrapMethod(c, wrapped, mi);
                            }
                            break;
                        case MemberTypes.Property:
                            PropertyInfo pi = (PropertyInfo)info;
                            WrapProperty(c, wrapped, pi);
                            break;
                        default:
                            Console.WriteLine(info + " : " + info.MemberType);
                            break;
                    }
                }
            }
            return c;
        }

        private static void WrapMethod(IClass t, IOperand wrapped, MethodInfo mi)
        {
            var method = t.Method(mi).Public;
            using (var code = method.Code())
            {
                code.Return(wrapped.Invoke(mi, method.Args.AsOperands()));
            }
        }

        private static void WrapProperty(IClass t, IOperand wrapped, PropertyInfo pi)
        {
            var property = t.Property(pi).Public;
            if (pi.CanRead)
            {
                var getter = property.Getter();
                using (var c = getter.Code())
                {
                    c.Return(wrapped.Property(pi, getter.Args.AsOperands()));
                }
            }
            if (pi.CanWrite)
            {
                var setter = property.Setter();
                using (var c = setter.Code())
                {
                    c.Assign(wrapped.Property(pi, setter.Args.AsOperands()), setter.Value);
                }
            }
        }
    }
}
