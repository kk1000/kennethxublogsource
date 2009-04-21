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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Spring.Data.Common
{
    /// <summary>
    /// Test cases for <see cref="ExtendedDbProvider"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class ExtendedDbProviderTest
    {
        private MockRepository _mockery;
        private IDbConnectionStateListener _listener;
        private IDbProvider _dbProvider;
        private ExtendedDbProvider _testee;

        [SetUp]
        public void SetUp()
        {
            _testee = new ExtendedDbProvider();
            _mockery = new MockRepository();
            _listener = _mockery.CreateMock<IDbConnectionStateListener>();
            _dbProvider = _mockery.CreateMock<IDbProvider>();
            _testee.TargetDbProvider = _dbProvider;
        }

        [Test] public void WorksFineWithoutListenerWhenDdConnection()
        {
            var connection = _mockery.CreateMock<DbConnection>();
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            ((IDbConnection)connection).Open();
            connection.StateChange += null;
            var eventRaiser = LastCall.IgnoreArguments().GetEventRaiser();
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            eventRaiser.Raise(connection, new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
            _mockery.VerifyAll();
        }

        [Test] public void WorksFineWithoutListenerWhenNotDdConnection()
        {
            var connection = _mockery.CreateMock<IDbConnection>();
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            Expect.Call(connection.Open);
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            _mockery.VerifyAll();
        }

        [Test] public void CallsListenerWhenDbConnection()
        {
            var connection = _mockery.CreateMock<DbConnection>();
            _testee.ConnectionStateListener = _listener;
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            ((IDbConnection)connection).Open();
            Expect.Call(() => _listener.AfterStateChange(connection, ConnectionState.Closed, ConnectionState.Open));
            connection.StateChange += null;
            var eventRaiser = LastCall.IgnoreArguments().GetEventRaiser();
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            eventRaiser.Raise(connection, new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
            _mockery.VerifyAll();
        }

        [Test] public void WorksFineWithListenerWhenNotDdConnection()
        {
            _testee.ConnectionStateListener = _listener;
            var connection = _mockery.CreateMock<IDbConnection>();
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            Expect.Call(connection.Open);
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            _mockery.VerifyAll();
        }
    }
}
