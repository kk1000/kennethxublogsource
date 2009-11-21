using NUnit.Framework;

namespace NUnitStuff
{
    public abstract class AbstractFixture
    {
        [Test] public void TestOnAbstractFixture() { }
        [Test] public virtual void OverrideableTestOnAbstractFixture() { }
    }
}
