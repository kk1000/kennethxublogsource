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
using NUnit.Framework;
using Oracle.DataAccess.Client;
using Rhino.Mocks;
using Spring.Dao;
using Spring.Data.Common;
using Spring.Data.Support;

namespace Spring.Data.Generic
{
    /// <summary>
    /// Test cases for <see cref="OracleOdpTemplate"/>
    /// </summary>
    [TestFixture] public class OracleOdpTemplateTest
    {
        private MockRepository _mockery;
        private IDictionary<string, int> _dataSaved;
        private const string _sql = "fake sql statement";
        private const CommandType _cmdType = CommandType.Text;
        private Converter<string, IDbParameters> _converter;
        private OracleOdpTemplate _testee;
        private const int _batchSize = 5;
        readonly IDbProvider _dbProvider = DbProviderFactory.GetDbProvider("Oracle.DataAccess.Client");

        [SetUp] public void SetUp()
        {
            _testee = new OracleOdpTemplate();
            _mockery = new MockRepository();
            _converter = _mockery.CreateMock<Converter<string, IDbParameters>>();
            _dataSaved = new Dictionary<string, int>();
        }

        [Test] public void DefaultBatchSizeIsSet()
        {
            Assert.That(_testee.BatchSize, Is.EqualTo(OracleOdpTemplate.DEFALT_BATCH_SIZE));
        }

        [Test] public void DataReaderWrapperIsSet()
        {
            Assert.That(_testee.DataReaderWrapperType, Is.EqualTo(typeof(OdpNetDataReaderWrapper)));
        }

        [Test] public void ExecuateNonQueryChokesWithUnmatchedParameterName()
        {
            Action<IDbParameters, int> action =
                delegate(IDbParameters parameters, int rowNum)
                    {
                        if (rowNum == 1)
                        {
                            parameters["string_field"].ParameterName = "bad_field";
                        }
                    };
            Assert.Throws<InvalidDataAccessApiUsageException>(
                () => RunExecuateNonQueryBatch(2, 1, action));
        }

        [Test] public void ExecuateNonQueryChokesWithLessParameters()
        {
            Action<IDbParameters, int> action =
                delegate(IDbParameters parameters, int rowNum)
                    {
                        if (rowNum == 1)
                        {
                            parameters.DataParameterCollection.RemoveAt(1);
                        }
                    };
            Assert.Throws<InvalidDataAccessApiUsageException>(
                () => RunExecuateNonQueryBatch(2, 1, action));
        }

        [Test] public void ExecuateNonQueryChokesWithMoreParameters()
        {
            Action<IDbParameters, int> action =
                delegate(IDbParameters parameters, int rowNum)
                    {
                        if (rowNum == 1)
                        {
                            parameters.Add("bad_field", DbType.Decimal);
                        }
                    };
            Assert.Throws<InvalidDataAccessApiUsageException>(
                () => RunExecuateNonQueryBatch(2, 1, action));
        }

        [Test] public void ExecuateNonQueryHalfBatch()
        {
            RunExecuateNonQueryBatch(_batchSize/2, 1, null);
        }

        [Test] public void ExecuateNonQueryFullBatch()
        {
            RunExecuateNonQueryBatch(_batchSize, 1, null);
        }

        [Test] public void ExecuateNonQueryTwoBatches()
        {
            RunExecuateNonQueryBatch(_batchSize * 2, 2, null);
        }

        [Test] public void ExecuateNonQueryTwoAndHalfBatches()
        {
            RunExecuateNonQueryBatch(_batchSize * 5 / 2, 3, null);
        }

        void RunExecuateNonQueryBatch(int sampleSize, int repeat, Action<IDbParameters, int> action)
        {
            var mock = _mockery.CreateMock<IAdoOperations>();
            Expect.Call(mock.ExecuteNonQuery(CommandType.Text, _sql, (ICommandSetter) null)).Return(3).Repeat.Times(repeat)
                .Callback(new Func<CommandType, string, ICommandSetter, bool>(ExecuteNonQueryCallback));
            _testee.DbProvider = _dbProvider;
            _testee.BatchSize = _batchSize;

            var sampleData = new Dictionary<string, int>();
            for(int i=0; i<sampleSize; i++)
            {
                string key = "string " + i;
                IDbParameters parameters = new DbParameters(_dbProvider);
                parameters.Add("string_field", DbType.String).Value = key;
                parameters.Add("int_field", DbType.Int32).Value = i;
                if (action != null) action(parameters, i);
                Expect.Call(_converter(key)).Return(parameters);
                sampleData.Add(key, i);
            }

            _mockery.ReplayAll();
            int result = _testee.GetExecutor().ExecuteNonQuery(mock, _cmdType, _sql, sampleData.Keys, _converter);
            Assert.That(result, Is.EqualTo(3 * repeat));
            CollectionAssert.AreEqual(sampleData, _dataSaved);
            _mockery.VerifyAll();
        }

        private bool ExecuteNonQueryCallback(CommandType cmdType, string cmdText, ICommandSetter setter)
        {
            if (cmdType != _cmdType) return false;
            if (cmdText != _sql) return false;
            OracleCommand command = new OracleCommand();
            setter.SetValues(command);
            object[] stringFields = (object[])command.Parameters["string_field"].Value;
            object[] intFields = (object[])command.Parameters["int_field"].Value;
            for (int i = 0; i < command.ArrayBindCount; i++)
            {
                _dataSaved.Add((string)stringFields[i], (int)intFields[i]);
            }
            return true;
        }
    }
}