#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections;
using Rhino.Mocks;

namespace NUnitStuff
{
    /// <summary>
    /// Implementation of <see cref="ITestDataProvider"/> that uses
    /// RhinoMocks to create mock objects.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class RhinoMocksTestDataProvider : ITestDataProvider
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
        public IEnumerable MakeDataPoints(Type type)
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
