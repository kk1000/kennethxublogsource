using NUnit.Framework;

namespace NUnitStuff
{
    public abstract class AbstractFixture : ThirdPartyFixture
    {
        [Test] public void TestOnAbstractFixture() { }
        [Test] public virtual void OverrideableTestOnAbstractFixture() { }
    }
}
