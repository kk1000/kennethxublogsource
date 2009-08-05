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
using System.Data.Common;
using System.Linq;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using Rhino.Mocks;

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
        private InMemoryLoggerFactoryAdapter _loggerFactory;
        private InMemoryLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = (InMemoryLoggerFactoryAdapter)LogManager.Adapter;
            _logger = _loggerFactory.GetInMemoryLogger(typeof(ExtendedDbProvider));
            _testee = new ExtendedDbProvider();
            _mockery = new MockRepository();
            _listener = _mockery.StrictMock<IDbConnectionStateListener>();
            _dbProvider = _mockery.StrictMock<IDbProvider>();
            _testee.TargetDbProvider = _dbProvider;
        }

        [Test] public void WorksFineWithoutListenerWhenConnectionIsDdConnection()
        {
            var connection = _mockery.StrictMock<DbConnection>();
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            ((IDbConnection)connection).Open();
            connection.StateChange += null;
            var eventRaiser = LastCall.IgnoreArguments().Repeat.Any().GetEventRaiser();
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            eventRaiser.Raise(connection, new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
            _mockery.VerifyAll();
        }

        [Test] public void WorksFineWithoutListenerWhenConnectionIsNotDdConnection()
        {
            var connection = _mockery.StrictMock<IDbConnection>();
            Expect.Call(_dbProvider.CreateConnection()).Return(connection);
            Expect.Call(connection.Open);
            _mockery.ReplayAll();
            var conn = _testee.CreateConnection();
            conn.Open();
            _mockery.VerifyAll();
        }

        [Test] public void CallsListenerWhenConnectionIsDbConnection()
        {
            var connection = _mockery.StrictMock<DbConnection>();
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

        private bool IsNonDbConnectionErrorMessage(string message)
        {
            return message.Contains(typeof(DbConnection).FullName) &&
                   message.Contains(_listener.ToString()) &&
                   message.Contains(_dbProvider.ToString());
        }

        [Test] public void LogErrorOnceWithListenerWhenConnectionIsNotDdConnection()
        {
            _logger.Level = LogLevel.Error;
            _testee.ConnectionStateListener = _listener;
            Expect.Call(_dbProvider.CreateConnection()).Return(_mockery.Stub<IDbConnection>()).Repeat.Twice();
            _mockery.ReplayAll();
            _testee.CreateConnection().Open();
            _testee.CreateConnection().Open();
            var query = from entry in _logger.LogEntries
                        where IsNonDbConnectionErrorMessage(entry.Message.ToString())
                        select entry;
            Assert.That(query.Count(), Is.EqualTo(1));
            Assert.That(query.First().LogLevel, Is.EqualTo(LogLevel.Error));

            _mockery.VerifyAll();
        }

        
        public static void aaa()
        {
            object o = new object();
            ((ILog)o).Info(o);
        }
    }
}
