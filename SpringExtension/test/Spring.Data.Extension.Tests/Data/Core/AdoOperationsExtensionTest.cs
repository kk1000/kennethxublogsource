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
using Rhino.Mocks;
using Spring.Data.Common;
using NUnit.Framework;
using Spring.Data.Support;

namespace Spring.Data.Core
{
    /// <summary>
    /// Test cases for <see cref="AdoOperationsExtension"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class AdoOperationsExtensionTest
    {
        private MockRepository _mockery;
        private IAdoOperations _adoOperations;
        private const string _sql = "fake sql statement";
        private IDataRecordOrdinalCache _ordinalCache;
        private int _rowsExpected;
        private IRowCallback _rowCallback;
        private RowCallbackDelegate _rowCallbackDelegate;

        [SetUp]
        public void SetUp()
        {
            _mockery = new MockRepository();
            _adoOperations = _mockery.CreateMock<IAdoOperations>();
            _ordinalCache = _mockery.CreateMock<IDataRecordOrdinalCache>();
            _rowCallback = _mockery.CreateMock<IRowCallback>();
            _rowCallbackDelegate = _mockery.CreateMock<RowCallbackDelegate>();
        }

        #region QueryWithRowCallback

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallback(CommandType commandType, int rowsExpected)
        {
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, bool>(
                              (cmdType, sql, extractor) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallback(
                commandType, _sql, _rowCallback, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallbackWithCommandSetter(CommandType commandType, int rowsExpected)
        {
            var setter = _mockery.CreateMock<ICommandSetter>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null, setter))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, ICommandSetter, bool>(
                              (cmdType, sql, extractor, cmdSetter) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(cmdSetter, setter) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallback(
                commandType, _sql, _rowCallback, setter, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallbackWithDbParameters(CommandType commandType, int rowsExpected)
        {
            var parameters = _mockery.CreateMock<IDbParameters>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null, parameters))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, IDbParameters, bool>(
                              (cmdType, sql, extractor, dbParameters) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(dbParameters, parameters) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallback(
                commandType, _sql, _rowCallback, parameters, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1, DbType.String, 0, "string value")]
        [TestCase(CommandType.StoredProcedure, 10, DbType.Single, 2, 33.33)]
        public void QueryWithRowCallbackWithOneParameter(
            CommandType commandType, int rowsExpected, Enum dbType, int size, object value)
        {
            const string paramName = "paramName";
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(
                    CommandType.Text, _sql, null, paramName, dbType, size, value))
                .Return(null)
                .Callback(new OneParameterCall(
                              (cmdType, sql, extractor, theName, theType, theSize, theValue) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ReferenceEquals(theName, paramName) &&
                                  dbType == theType &&
                                  size == theSize &&
                                  value.Equals(theValue) &&
                                  ValidateExtendedRowCallbackResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallback(
                commandType, _sql, _rowCallback, paramName, dbType, size, value, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        private bool ValidateExtendedRowCallbackResultSetExtractor(IResultSetExtractor extractor)
        {
            var extended = extractor as ExtendedRowCallbackResultSetExtractor;
            return extended != null &&
                   extended.RowsExpected == _rowsExpected &&
                   ReferenceEquals(extended.OrdinalCache, _ordinalCache) &&
                   ReferenceEquals(extended._rowCallback, _rowCallback);
        }

        #endregion

        #region QueryWithRowCallbackDelegate

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallbackDelegate(CommandType commandType, int rowsExpected)
        {
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, bool>(
                              (cmdType, sql, extractor) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallbackDelegate(
                commandType, _sql, _rowCallbackDelegate, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallbackDelegateWithCommandSetter(CommandType commandType, int rowsExpected)
        {
            var setter = _mockery.CreateMock<ICommandSetter>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null, setter))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, ICommandSetter, bool>(
                              (cmdType, sql, extractor, cmdSetter) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(cmdSetter, setter) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallbackDelegate(
                commandType, _sql, _rowCallbackDelegate, setter, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowCallbackDelegateWithDbParameters(CommandType commandType, int rowsExpected)
        {
            var parameters = _mockery.CreateMock<IDbParameters>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(CommandType.Text, _sql, null, parameters))
                .Return(null)
                .Callback(new Func<CommandType, string, IResultSetExtractor, IDbParameters, bool>(
                              (cmdType, sql, extractor, dbParameters) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(dbParameters, parameters) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowCallbackDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallbackDelegate(
                commandType, _sql, _rowCallbackDelegate, parameters, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1, DbType.String, 0, "string value")]
        [TestCase(CommandType.StoredProcedure, 10, DbType.Single, 2, 33.33)]
        public void QueryWithRowCallbackDelegateWithOneParameter(
            CommandType commandType, int rowsExpected, Enum dbType, int size, object value)
        {
            const string paramName = "paramName";
            Expect.Call(_adoOperations.QueryWithResultSetExtractor(
                    CommandType.Text, _sql, null, paramName, dbType, size, value))
                .Return(null)
                .Callback(new OneParameterCall(
                              (cmdType, sql, extractor, theName, theType, theSize, theValue) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ReferenceEquals(theName, paramName) &&
                                  dbType == theType &&
                                  size == theSize &&
                                  value.Equals(theValue) &&
                                  ValidateExtendedRowCallbackDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            _adoOperations.QueryWithRowCallbackDelegate(
                commandType, _sql, _rowCallbackDelegate, paramName, dbType, size, value, _ordinalCache, rowsExpected);
            _mockery.VerifyAll();
        }

        private bool ValidateExtendedRowCallbackDelegateResultSetExtractor(IResultSetExtractor extractor)
        {
            var extended = extractor as ExtendedRowCallbackResultSetExtractor;
            return extended != null &&
                   extended.RowsExpected == _rowsExpected &&
                   ReferenceEquals(extended.OrdinalCache, _ordinalCache) &&
                   ReferenceEquals(extended._rowCallbackDelegate, _rowCallbackDelegate);
        }

        #endregion

        private delegate bool OneParameterCall(CommandType cmdType, string cmdText, IResultSetExtractor extractor, string paramName, Enum dbType, int size, object value);
    }
}
