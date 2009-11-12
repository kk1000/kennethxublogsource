using System;
using System.Collections;

namespace NUnitStuff
{
    /// <summary>
    /// Define a contract to plugin data point generation used by
    /// <see cref="ValueObjectTestFixture{T}.TestData"/>.
    /// </summary>
    /// <remarks>
    /// This is typically implemented by using a mock framework.
    /// </remarks>
    /// <seealso cref="ValueObjectTestFixture{T}.TestDataProvider"/>
    /// <author>Kenneth Xu</author>
    public interface ITestDataProvider
    {
        /// <summary>
        /// Generates test object of a given type and return them in an
        /// <see cref="IEnumerable"/>. Implementation must make sure the
        /// second data point in the eumerable must be null for reference
        /// type.
        /// </summary>
        /// <param name="type">
        /// The type of the object to be created.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> of generated objects of
        /// <paramref name="type"/>, where the second element must be null
        /// if it is reference type. Or return null if the given
        /// <paramref name="type"/>.
        /// </returns>
        IEnumerable MakeDataPoints(Type type);
    }
}