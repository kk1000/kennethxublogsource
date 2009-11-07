using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;
using CodeSharp.Emit;

namespace CodeSharp
{
    public interface IIndexerProperty
    {
        long this[int i, string s] { get; set; }
    }

    [TestFixture]
    public class IndexerPropertyWrapperTest
    {
        private static readonly Type _interface = typeof(IIndexerProperty);
        private ModuleBuilder _moduleBuilder;
        private IIndexerProperty _mock;
        private Emitter _emmiter;
        private int _i = 12355;
        private string _s = "f#@$f";
        const string _moduleName = "IndexerPropertyWrapper.dll";
        private const string _namespace = _moduleName;
        private const string _wrappedFieldName = "_wrapped";
        const long _getValue = 93858;
        private const long _setValue = 491743;

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
            _mock = MockRepository.GenerateMock<IIndexerProperty>();
            _mock.Stub(x => x[_i, _s]).Return(_getValue);
        }

        [Test]
        public void InvokeChokesOnNonExistentMethod()
        {
            var g = _emmiter;
            IClass c = _emmiter.Class("Nonexistent").In(_namespace);
            {
                var f = c.Field(_interface, _wrappedFieldName);

                var p = c.Indexer(typeof(long), g.Arg<int>("i"), g.Arg<string>("s")).Public;
                var m = p.Getter();
                using (m.Code())
                {
                    var e = Assert.Throws<ArgumentException>(() => f.Indexer(m.Arg[0]));
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
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
            var indexerProperty = t.GetProperty("Item");
            Assert.That(indexerProperty.GetValue(sut, new object[]{_i, _s}), Is.EqualTo(_getValue));
            _mock.AssertWasCalled(x => x[_i, _s]);
            indexerProperty.SetValue(sut, _setValue, new object[] { _i, _s });
            _mock.AssertWasCalled(x => x[_i, _s] = _setValue);
        }

        [Test]
        public void ImplementingWrapper()
        {
            IClass c = _emmiter.Class("Implemening").In(_namespace).Implements(_interface);
            {
                CreateClassMembers(c, _emmiter);
            }
            var t = _emmiter.Generate(c);
            var sut = (IIndexerProperty)t.GetConstructor(new[] { _interface }).Invoke(new object[] { _mock });
            Assert.That(sut[_i, _s], Is.EqualTo(_getValue));
            sut[_i, _s] = _setValue;
            _mock.AssertWasCalled(x => x[_i, _s] = _setValue);
            _mock.AssertWasCalled(x => x[_i, _s]);
        }

        private static void CreateClassMembers(IClass t, IGenerator g)
        {
            var wrapped = t.Field(_interface, _wrappedFieldName);

            var ctor = t.Constructor(g.Arg<IIndexerProperty>("wrapped")).Public;
            using (var c = ctor.Code())
            {
                c.Assign(wrapped, ctor.Arg[0]);
            }

            var indexerProperty = t.Indexer(typeof(long), g.Arg<int>("i"), g.Arg<string>("s")).Public;
            var getter = indexerProperty.Getter();
            using (var c = getter.Code())
            {
                c.Return(wrapped.Indexer(getter.Arg[0], getter.Arg[1]));
            }
            var setter = indexerProperty.Setter();
            using (var c = setter.Code())
            {
                c.Assign(wrapped.Indexer(getter.Arg[0], getter.Arg[1]), setter.Value);
            }
        }
    }
}
