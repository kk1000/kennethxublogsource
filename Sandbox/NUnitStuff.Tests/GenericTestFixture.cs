using NUnit.Framework;

namespace NUnitStuff
{
    [TestFixture(typeof(int))]
    [TestFixture(typeof(string))]
    public class GenericTestFixture<T> : ThirdPartyFixture
    {
        [Test] public void TestOnGenericFixture()
        {
        }
    }
}
