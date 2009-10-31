using System;
using System.Collections;

namespace NUnitStuff
{
    /// <summary>
    /// Define a contract to plugin data point generation used by
    /// <see cref="ValueObjectTestFixture{T}.TestData"/>.
    /// </summary>
    /// <seealso cref="ValueObjectTestFixture{T}.MockProvider"/>
    public interface IMockProvider
    {
        /// <summary>
        /// Generates mock objects of a given type and return them in an
        /// <see cref="IEnumerable"/>. Implementation must make sure the
        /// second data point in the eumerable must be null for reference
        /// type.
        /// </summary>
        /// <param name="type">
        /// The type of the object to be mocked.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> of generated mock object of
        /// <paramref name="type"/>, where the second element must be null
        /// if it is reference type. Or return null if the given
        /// <paramref name="type"/>.
        /// </returns>
        IEnumerable MockDataPoints(Type type);
    }
}