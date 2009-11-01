using System;
using System.Collections;
using Rhino.Mocks;

namespace NUnitStuff
{
    /// <summary>
    /// Implementation of <see cref="IMockTestDataProvider"/> that uses
    /// RhinoMocks to create mock objects.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class RhinoMockTestDataProvider : IMockTestDataProvider
    {
        readonly MockRepository _repository = new MockRepository();

        /// <summary>
        /// Generates mock objects of a given type and return them in an
        /// <see cref="IEnumerable"/>.
        /// </summary>
        /// <remarks>
        /// This implementation support all interfaces, and classes that
        /// are not sealed and with default constructor.
        /// </remarks>
        /// <param name="type">
        /// The type of the object to be mocked.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> of generated mock object of
        /// <paramref name="type"/>, where the second element is null
        /// if it is reference type. Or return null if the given
        /// <paramref name="type"/>.
        /// </returns>
        public IEnumerable MockDataPoints(Type type)
        {
            if (!type.IsInterface && 
                (type.IsValueType||type.IsSealed||type.GetConstructor(Type.EmptyTypes) == null)) 
                return null;
            return new []{CreateMock(type), null, CreateMock(type)};
        }

        private object CreateMock(Type type)
        {
            var mock = _repository.DynamicMock(type);
            mock.Replay();
            return mock;
        }
    }
}
