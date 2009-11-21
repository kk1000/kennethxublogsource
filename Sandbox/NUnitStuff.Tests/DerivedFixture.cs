using NUnit.Framework;

namespace NUnitStuff
{
    public class DerivedFixture : AbstractFixture
    {
        [Test] public void TestOnDerivedFixture() { }
        public override void OverrideableTestOnAbstractFixture() { }
    }
}