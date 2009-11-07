namespace NUnitStuff
{
    /// <summary>
    /// Base test cases for value objects that has a default constructor.
    /// </summary>
    /// <typeparam name="T">Type of the value object to be tested.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class NewableValueObjectTestFixture<T> : ValueObjectTestFixture<T>
        where T : new()
    {
        /// <summary>
        /// Create a new value object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        protected override T NewValueObject()
        {
            return new T();
        }
    }
}
