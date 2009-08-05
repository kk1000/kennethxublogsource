#region License

/*
 * Copyright (C) 2009 the original author or authors.
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

using System.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Spring.Data.Common
{
    /// <summary>
    /// Test cases for <see cref="DbConnectionStateListenerBase"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class DbConnectionStateListenerBaseTest
    {
        private MockRepository _mockery;
        private DbConnectionStateListenerBase _listener;
        private IDbConnection _connection;
        [SetUp]
        public void SetUp()
        {
            _mockery = new MockRepository();
            _listener = _mockery.StrictMock<DbConnectionStateListenerBase>();
            _connection = _mockery.StrictMock<IDbConnection>();
        }

        [Test] public void WhenConnectionOpen()
        {
            _listener.AfterStateChange(_connection, ConnectionState.Closed, ConnectionState.Open);
            LastCall.CallOriginalMethod(OriginalCallOptions.NoExpectation);
            _listener.AfterConnectionOpen(_connection);
            LastCall.CallOriginalMethod(OriginalCallOptions.CreateExpectation);
            _mockery.ReplayAll();
            _listener.AfterStateChange(_connection, ConnectionState.Closed, ConnectionState.Open);
            _mockery.VerifyAll();
        }

        [Test] public void WhenConnectionClose()
        {
            _listener.AfterStateChange(_connection, ConnectionState.Closed, ConnectionState.Open);
            LastCall.CallOriginalMethod(OriginalCallOptions.NoExpectation);
            _listener.AfterConnectionClose(_connection);
            LastCall.CallOriginalMethod(OriginalCallOptions.CreateExpectation);
            _mockery.ReplayAll();
            _listener.AfterStateChange(_connection, ConnectionState.Open, ConnectionState.Closed);
            _mockery.VerifyAll();
        }

        [Test] public void WhenConnectionNeitherOpenNorClose()
        {
            _listener.AfterStateChange(_connection, ConnectionState.Open, ConnectionState.Open);
            LastCall.CallOriginalMethod(OriginalCallOptions.NoExpectation).Repeat.Any();
            _mockery.ReplayAll();
            _listener.AfterStateChange(_connection, ConnectionState.Open, ConnectionState.Fetching);
            _listener.AfterStateChange(_connection, ConnectionState.Fetching, ConnectionState.Open);
            _listener.AfterStateChange(_connection, ConnectionState.Open, ConnectionState.Broken);
            _listener.AfterStateChange(_connection, ConnectionState.Fetching, ConnectionState.Broken);
            _mockery.VerifyAll();
        }
    }
}
