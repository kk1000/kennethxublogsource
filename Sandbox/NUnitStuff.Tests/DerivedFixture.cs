using NUnit.Framework;

namespace NUnitStuff
{
    public class DerivedFixture : AbstractFixture
    {
        [Test] public void TestOnDerivedFixture(int i) { }
        public override void OverrideableTestOnAbstractFixture() { }
    }
}