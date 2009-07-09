using NUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Extension.Test
{
    [TestFixture] public class NewAaaSyntaxTest
    {
        public interface IFoo
        {
            void Foo(int i);
            void Foo(int i, int j);
            void Foo(int i, int j, int k); 
        }

        [Test] public void Or_succeed_when_any_of_the_methods_are_called()
        {
            var foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1);
            AssertAny(foo);

            foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1, 0);
            AssertAny(foo);

            foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1, 10);
            AssertAny(foo);
        }

        private void AssertAny(IFoo foo)
        {
            Assert.IsTrue(foo.ActivityOf(f => f.Foo(1)) || 
                          foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything)));

            Mockery.Assert(foo.ActivityOf(f => f.Foo(1))
                               .Or(foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything))));

            Mockery.Assert(foo.ActivityOf(f => f.Foo(1)) | 
                           foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything)));

            (foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything)) | 
             foo.ActivityOf(f => f.Foo(1))).AssertOccured();

            // C# 2.0 syntax
            Mockery.Assert(Mockery.ActivityOf(foo, delegate(IFoo f) { f.Foo(1); }) | 
                           Mockery.ActivityOf(foo, delegate(IFoo f) { f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything); }));
        }

        [Test] public void Or_failed_when_non_of_the_methods_is_called()
        {
            var foo = MockRepository.GenerateMock<IFoo>();
            Assert.IsFalse(foo.ActivityOf(f => f.Foo(1)) || 
                           foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything)));

            Assert.Throws<ExpectationViolationException>(
                () => Mockery.Assert(foo.ActivityOf(f => f.Foo(1))
                                         .Or(foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything)))));

            Assert.Throws<ExpectationViolationException>(
                () => Mockery.Assert(foo.ActivityOf(f => f.Foo(1)) |
                                     foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything))));

            Assert.Throws<ExpectationViolationException>(
                () => (foo.ActivityOf(f => f.Foo(1)) | 
                       foo.ActivityOf(f => f.Foo(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything))).AssertOccured());
        }

        [Test] public void OneOf_succeed_when_one_and_only_one_is_called()
        {
            var 
                foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1);
            CheckOneOf(foo, true);

            foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1, 2);
            CheckOneOf(foo, true);

            foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1, 2, 3);
            CheckOneOf(foo, true);
        }

        [Test]
        public void OneOf_fail_when_more_then_one_is_called()
        {
            var
                foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1);
            foo.Foo(1);
            CheckOneOf(foo, false);

            foo = MockRepository.GenerateMock<IFoo>();
            foo.Foo(1, 2, 3);
            foo.Foo(1, 2);
            CheckOneOf(foo, false);
        }

        [Test]
        public void OneOf_fail_when_none_is_called()
        {
            var foo = MockRepository.GenerateMock<IFoo>();
            CheckOneOf(foo, false);
        }

        private void CheckOneOf(IFoo foo, bool succeed)
        {
            var expect = Mockery.ExactOneOf(
                foo.ActivityOf(f=>f.Foo(1), m=>m.Repeat.Once()),
                foo.ActivityOf(f=>f.Foo(1,2), m=>m.Repeat.Once()),
                foo.ActivityOf(f=>f.Foo(1,2,3), m=>m.Repeat.Once())
                );
            if (succeed)
            {
                Assert.IsTrue(expect);
                Mockery.Assert(expect);
            }
            else
            {
                Assert.IsFalse(expect);
                Assert.Throws<ExpectationViolationException>(
                    () => Mockery.Assert(expect));
            }
        }
    }
}