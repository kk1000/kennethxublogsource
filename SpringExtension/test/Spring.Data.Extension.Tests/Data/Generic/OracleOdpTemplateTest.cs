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
using System.Text;
using NUnit.Framework;
using Oracle.DataAccess.Client;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Spring.Dao;
using Spring.Data;
using Spring.Data.Common;
using Spring.Data.Generic;
using Spring.Data.Support;
using IAdoOperations=Spring.Data.Generic.IAdoOperations;

namespace Spring.Extension.Tests.Data.Generic
{
    /// <summary>
    /// Test cases for <see cref="OracleOdpTemplate"/>
    /// </summary>
    [TestFixture] public class OracleOdpTemplateTest
    {
        private MockRepository _mockery;
        private const string _sql = "fake sql statement";
        private Converter<string, IDbParameters> _converter;
        private OracleOdpTemplate _testee;
        private int _batchSize = 5;
        IDbProvider dbProvider = DbProviderFactory.GetDbProvider("Oracle.DataAccess.Client");


        [SetUp] public void SetUp()
        {
            _testee = new OracleOdpTemplate();
            _mockery = new MockRepository();
            _converter = _mockery.CreateMock<Converter<string, IDbParameters>>();
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
            var stub = new OracleOdpTemplateStub();
            stub.ExpectedCallExecuteNonQuery(CommandType.Text, _sql, null).WillReturn(3).RepeatTimes(repeat);
            _testee = stub;
            _testee.DbProvider = dbProvider;
            _testee.BatchSize = _batchSize;

            var sampleData = new Dictionary<string, int>();
            for(int i=0; i<sampleSize; i++)
            {
                string key = "string " + i;
                IDbParameters parameters = new DbParameters(dbProvider);
                parameters.Add("string_field", DbType.String).Value = key;
                parameters.Add("int_field", DbType.Int32).Value = i;
                if (action != null) action(parameters, i);
                Expect.Call(_converter(key)).Return(parameters);
                sampleData.Add(key, i);
            }

            _mockery.ReplayAll();
            int result = _testee.GetExecutor().ExecuteNonQuery(_testee, CommandType.Text, _sql, sampleData.Keys, _converter);
            Assert.That(result, Is.EqualTo(3 * repeat));
            CollectionAssert.AreEqual(sampleData, stub.DataSaved);
            _mockery.VerifyAll();
            stub.VerifyAll();
        }

        private class OracleOdpTemplateStub : OracleOdpTemplate, IAdoOperations
        {
            const string EXPECTATION_MESSAGE =
                        "IAdoOperations.ExecuteNonQuery(CommandType, string, ICommandSetter); Expected #{0}, Actual #{1}.";
            private CommandType _expectedCommandType;
            private string _expectedCommandText;
            private int _returnValue, _repeat = 0, _callCount = 0;

            private readonly IDictionary<string, int> _dataSaved = new Dictionary<string, int>();

            internal IDictionary<string, int> DataSaved { get { return _dataSaved; } }

            internal OracleOdpTemplateStub ExpectedCallExecuteNonQuery(CommandType cmdType, string cmdText, ICommandSetter setter)
            {
                _expectedCommandType = cmdType;
                _expectedCommandText = cmdText;
                _repeat++;
                return this;
            }

            internal OracleOdpTemplateStub WillReturn(int returnValue)
            {
                _returnValue = returnValue;
                return this;
            }

            internal OracleOdpTemplateStub RepeatTimes(int repeat)
            {
                _repeat += repeat-1;
                return this;
            }

            internal void VerifyAll()
            {
                if (_repeat != _callCount)
                {
                    throw new ExpectationViolationException(string.Format(EXPECTATION_MESSAGE, _repeat, _callCount));
                }
            }

            public new int ExecuteNonQuery(CommandType cmdType, string cmdText, ICommandSetter setter)
            {
                if (++_callCount > _repeat)
                {
                    throw new ExpectationViolationException(string.Format(EXPECTATION_MESSAGE, _repeat, _callCount));
                }
                Assert.AreEqual(_expectedCommandType, cmdType);
                Assert.AreEqual(_expectedCommandText, cmdText);
                OracleCommand command = new OracleCommand();
                setter.SetValues(command);
                object[] stringFields = (object[])command.Parameters["string_field"].Value;
                object[] intFields = (object[])command.Parameters["int_field"].Value;
                for (int i = 0; i < command.ArrayBindCount; i++)
                {
                    _dataSaved.Add((string)stringFields[i], (int)intFields[i]);
                }
                return _returnValue;
            }
        }
    }

}
