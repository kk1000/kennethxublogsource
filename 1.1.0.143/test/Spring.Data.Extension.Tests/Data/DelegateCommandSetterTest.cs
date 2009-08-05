using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Spring.Data
{
    [TestFixture] public class DelegateCommandSetterTest
    {
        [Test] public void ConstructorChokesOnNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new DelegateCommandSetter(null));
        }

        [Test] public void SetValueCallsTheDelegate()
        {
            var mockDelegate = MockRepository.GenerateStub<Action<IDbCommand>>();
            var mockDbCommand = MockRepository.GenerateStub<IDbCommand>();
            var sut = new DelegateCommandSetter(mockDelegate);
            sut.SetValues(mockDbCommand);
            mockDelegate.AssertWasCalled(d=>d(mockDbCommand));
        }
    }
}
