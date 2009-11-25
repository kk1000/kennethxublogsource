using NUnit.Framework;

namespace NUnitStuff
{
    //forgot [TestFixture(typeof(int))]
    //forgot [TestFixture(typeof(string))]
    public class GenericTestFixture<T> : ThirdPartyFixture
    {
        [Test] public void TestOnGenericFixture()
        {
        }
    }
}
