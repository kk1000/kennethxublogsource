using NUnit.Framework;

namespace NUnitStuff
{
    public class ParameterizedFixture
    {
        public ParameterizedFixture(int i) {}
        [Test] public void TestOnParameterizedFixture() {}
    }
}
