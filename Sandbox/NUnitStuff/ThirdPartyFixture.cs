using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// A bad test fixture example by 3rd party that has <see cref="TestFixtureAttribute"/>
    /// on abstract class.
    /// </summary>
    [TestFixture]
    public abstract class ThirdPartyFixture
    {
        /// <summary>
        /// And the fixture actually has some test in it.
        /// </summary>
        [Test] public void ThirdPartyTest() {}
    }

}
