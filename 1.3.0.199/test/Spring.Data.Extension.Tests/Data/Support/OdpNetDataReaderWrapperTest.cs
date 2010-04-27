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
using System.Data;
using System.Linq;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using Oracle.DataAccess.Client;
using Rhino.Mocks;

namespace Spring.Data.Support
{
    /// <summary>
    /// Test cases for <see cref="OdpNetDataReaderWrapper"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture]
    public class OdpNetDataReaderWrapperTest
    {
        private MockRepository _mockery;
        private OdpNetDataReaderWrapper _testee;
        private IDataReader _wrapped;
        private InMemoryLoggerFactoryAdapter _loggerFactory;
        private InMemoryLogger _logger;

        [SetUp] public void SetUp()
        {
            _loggerFactory = (InMemoryLoggerFactoryAdapter)LogManager.Adapter;
            _logger = _loggerFactory.GetInMemoryLogger(typeof(OdpNetDataReaderWrapper));
            _mockery = new MockRepository();
            _testee = new OdpNetDataReaderWrapper();
            _wrapped = _mockery.StrictMock<IDataReader>();
        }

        [TearDown] public void TearDown()
        {
            _loggerFactory.ClearAll();
        }

        [Test] public void GetCharMethod()
        {
            Expect.Call(_wrapped.GetString(1)).Return("A");
            Expect.Call(_wrapped.GetString(5)).Return("Error");
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            Assert.AreEqual('A', _testee.GetChar(1));

            Assert.Throws(Is.InstanceOf<Exception>(), () => _testee.GetChar(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetBooleanMethod()
        {
            Expect.Call(_wrapped.GetDataTypeName(1)).Return(OracleDbType.Char.ToString());
            Expect.Call(_wrapped.GetString(1)).Return("Y");
            Expect.Call(_wrapped.GetDataTypeName(2)).Return(OracleDbType.Varchar2.ToString());
            Expect.Call(_wrapped.GetString(2)).Return("N");

            Expect.Call(_wrapped.GetDataTypeName(3)).Return(OracleDbType.Decimal.ToString());
            Expect.Call(_wrapped.GetDecimal(3)).Return(new decimal(1.0));
            Expect.Call(_wrapped.GetDataTypeName(4)).Return(OracleDbType.Decimal.ToString());
            Expect.Call(_wrapped.GetDecimal(4)).Return(new decimal(0));

            Expect.Call(_wrapped.GetDataTypeName(5)).Return(OracleDbType.Double.ToString());
            Expect.Call(_wrapped.GetDouble(5)).Return(1.0D);
            Expect.Call(_wrapped.GetDataTypeName(6)).Return(OracleDbType.Double.ToString());
            Expect.Call(_wrapped.GetDouble(6)).Return(0.0D);

            Expect.Call(_wrapped.GetDataTypeName(7)).Return(OracleDbType.Single.ToString());
            Expect.Call(_wrapped.GetFloat(7)).Return(1.0f);
            Expect.Call(_wrapped.GetDataTypeName(8)).Return(OracleDbType.Single.ToString());
            Expect.Call(_wrapped.GetFloat(8)).Return(0.0f);

            Expect.Call(_wrapped.GetDataTypeName(9)).Return(OracleDbType.Int64.ToString());
            Expect.Call(_wrapped.GetInt64(9)).Return(1L);
            Expect.Call(_wrapped.GetDataTypeName(10)).Return(OracleDbType.Int64.ToString());
            Expect.Call(_wrapped.GetInt64(10)).Return(0L);

            Expect.Call(_wrapped.GetDataTypeName(11)).Return(OracleDbType.Int32.ToString());
            Expect.Call(_wrapped.GetInt32(11)).Return(1);
            Expect.Call(_wrapped.GetDataTypeName(12)).Return(OracleDbType.Int32.ToString());
            Expect.Call(_wrapped.GetInt32(12)).Return(0);

            Expect.Call(_wrapped.GetDataTypeName(13)).Return(OracleDbType.Int16.ToString());
            Expect.Call(_wrapped.GetInt16(13)).Return(1);
            Expect.Call(_wrapped.GetDataTypeName(14)).Return(OracleDbType.Int16.ToString());
            Expect.Call(_wrapped.GetInt16(14)).Return(0);

            Expect.Call(_wrapped.GetDataTypeName(15)).Return(OracleDbType.Blob.ToString());
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            Assert.IsTrue(_testee.GetBoolean(1));
            Assert.IsFalse(_testee.GetBoolean(2));
            Assert.IsTrue(_testee.GetBoolean(3));
            Assert.IsFalse(_testee.GetBoolean(4));
            Assert.IsTrue(_testee.GetBoolean(5));
            Assert.IsFalse(_testee.GetBoolean(6));
            Assert.IsTrue(_testee.GetBoolean(7));
            Assert.IsFalse(_testee.GetBoolean(8));
            Assert.IsTrue(_testee.GetBoolean(9));
            Assert.IsFalse(_testee.GetBoolean(10));
            Assert.IsTrue(_testee.GetBoolean(11));
            Assert.IsFalse(_testee.GetBoolean(12));
            Assert.IsTrue(_testee.GetBoolean(13));
            Assert.IsFalse(_testee.GetBoolean(14));
            Assert.Throws<InvalidCastException>(() => _testee.GetBoolean(15));
            _mockery.VerifyAll();
        }

        [Test] public void GiveWarningWhenNonOdpReaderOnlyOnce()
        {
            _logger.Level = LogLevel.Warn;
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            _testee.RowsExpected = 2313;
            _testee.RowsExpected = 948;
            var query = from entry in _logger.LogEntries
                        where entry.Message.ToString().Contains(
                            "Expected original reader to be " + typeof (OracleDataReader).FullName)
                        select entry;
            Assert.That(query.Count(), Is.EqualTo(1));
            Assert.That(query.First().LogLevel, Is.EqualTo(LogLevel.Warn));
            _mockery.VerifyAll();
        }

        [Test] public void SetRowsExpectedSetsFullBatchSizeOnOdpReader()
        {
            const int rowsExpected = 484;
            const int rowSize = 8389;
            OdpNetDataReaderWrapper.MaxFetchSize = int.MaxValue;
            SetRowsExpectedSetsBatchSizeWhenOdpReader(
                rowsExpected, rowSize, rowsExpected * rowSize);
        }

        [Test] public void SetRowsExpectedSetsHalfBatchSizeOnOdpReader()
        {
            const int rowsExpected = 484;
            const int rowSize = 8389;
            OdpNetDataReaderWrapper.MaxFetchSize = (rowsExpected * rowSize + 1) / 2;
            SetRowsExpectedSetsBatchSizeWhenOdpReader(
                rowsExpected, rowSize, OdpNetDataReaderWrapper.MaxFetchSize);
        }

        [Test, TestCaseSource("BatchSizeOptimizationCases")]
        public void BatchSizeOptimization(int maxSize, int rowsExpected, int rowSize, int fetchSize)
        {
            OdpNetDataReaderWrapper.MaxFetchSize = maxSize;
            SetRowsExpectedSetsBatchSizeWhenOdpReader(1, rowSize, fetchSize);
        }

        private void SetRowsExpectedSetsBatchSizeWhenOdpReader(int rowsExpected, int rowSize, int fetchSize)
        {
            _testee = _mockery.PartialMock<OdpNetDataReaderWrapper>();
            var wrapped = _mockery.StrictMock<OracleDataReader>();
            wrapped.FetchSize = fetchSize;
            Expect.Call(_testee.RowSize).Return(rowSize);
            _mockery.ReplayAll();
            _testee.WrappedReader = wrapped;
            _testee.RowsExpected = rowsExpected;
            Assert.That(_testee.RowsExpected, Is.EqualTo(rowsExpected));
            _mockery.VerifyAll();
        }

        private static object[] BatchSizeOptimizationCases =
            {
                new int[] {8, 1, 1, 1},
                new int[] {8, 1, 2, 2},
                new int[] {8, 1, 3, 3},
                new int[] {8, 1, 4, 4},
                new int[] {8, 1, 5, 5},
                new int[] {8, 1, 6, 6},
                new int[] {8, 1, 7, 7},
                new int[] {8, 1, 8, 8},
                new int[] {8, 1, 9, 5},
                new int[] {8, 1, 10, 5},
                new int[] {8, 1, 11, 6},
                new int[] {8, 1, 12, 6},
                new int[] {8, 1, 13, 7},
                new int[] {8, 1, 14, 7},
                new int[] {8, 1, 15, 8},
                new int[] {8, 1, 16, 8},
                new int[] {8, 1, 17, 6},
                new int[] {8, 1, 18, 6},
                new int[] {8, 1, 19, 7},
                new int[] {8, 1, 20, 7},
                new int[] {8, 1, 21, 7},
                new int[] {8, 1, 22, 8},
                new int[] {8, 1, 23, 8},
                new int[] {8, 1, 24, 8},
                new int[] {8, 1, 25, 7},
                new int[] {8, 1, 26, 7},
                new int[] {8, 1, 27, 7},
                new int[] {8, 1, 28, 7},
                new int[] {8, 1, 29, 8},
                new int[] {8, 1, 30, 8},
                new int[] {8, 1, 31, 8},
                new int[] {8, 1, 32, 8},
                new int[] {8, 1, 33, 7},
                new int[] {8, 1, 34, 7},
                new int[] {8, 1, 35, 7},
                new int[] {8, 1, 36, 8},
                new int[] {8, 1, 37, 8},
                new int[] {8, 1, 38, 8},
                new int[] {8, 1, 39, 8},
                new int[] {8, 1, 40, 8},
                new int[] {8, 1, 41, 7},
                new int[] {8, 1, 42, 7},
                new int[] {8, 1, 43, 8},
                new int[] {8, 1, 44, 8},
                new int[] {8, 1, 45, 8},
                new int[] {8, 1, 46, 8},
                new int[] {8, 1, 47, 8},
                new int[] {8, 1, 48, 8},
                new int[] {8, 1, 49, 7},
                new int[] {8, 1, 50, 8},
                new int[] {8, 1, 51, 8},
                new int[] {8, 1, 52, 8},
                new int[] {8, 1, 53, 8},
                new int[] {8, 1, 54, 8},
                new int[] {8, 1, 55, 8},
                new int[] {8, 1, 56, 8},
            };
    }
}