namespace NUnitStuff
{
    /// <summary>
    /// Base test cases for value objects that has a default constructor.
    /// </summary>
    /// <typeparam name="T">Type of the value object to be tested.</typeparam>
    /// <author>Kenneth Xu</author>
    public class NewableValueObjectTestFixture<T> : ValueObjectTestFixture<T>
        where T : new()
    {
        protected override T NewValueObject()
        {
            return new T();
        }
    }
}
