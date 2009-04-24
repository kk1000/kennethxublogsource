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
using Rhino.Mocks;
using Spring.Data.Common;
using NUnit.Framework;

namespace Spring.Data.Generic
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
        private Converter<string, IDbParameters> _converter;

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _adoOperations = _mockery.CreateMock<IAdoOperations>();
            _converter = _mockery.CreateMock<Converter<string, IDbParameters>>();
        }

        [Test] public void ExecutNonQueryChokesOnNullCommandText()
        {
            Exception e = Assert.Throws<ArgumentNullException>(() =>
                                                               AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, null, new string[1], _converter));
            StringAssert.Contains("cmdText", e.Message);
        }

        [Test] public void ExecutNonQueryChokesOnNullDataToParameters()
        {
            Exception e = Assert.Throws<ArgumentNullException>(() =>
                                                               AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, new string[1], null));
            StringAssert.Contains("dataToParameters", e.Message);
        }

        [Test] public void ExecutNonQueryReturnsZeroOnEmptyColleciton()
        {

            int result = AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, null, _converter);
            Assert.That(result, Is.EqualTo(0));
            result = AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, new string[0], _converter);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test] public void ExecuteNonQueryCallsBatchExecutor()
        {
            var collection = new[] {"x", "y", "c", "r"};
            _adoOperations = _mockery.CreateMultiMock<IAdoOperations>(typeof(IBatchExecutorFactory));
            var factory = (IBatchExecutorFactory)_adoOperations;
            var batchExecutor = _mockery.CreateMock<IBatchExecutor>();

            Expect.Call(factory.GetExecutor()).Return(batchExecutor);
            Expect.Call(batchExecutor.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, collection, _converter)).Return(10);

            _mockery.ReplayAll();
            int result = AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, collection, _converter);
            Assert.That(result, Is.EqualTo(10));
            _mockery.VerifyAll();

        }

        [Test] public void ExecuateNonQueryFallsBackToLoop()
        {
            var collection = new Dictionary<string, int>
                                 {
                                     {"a", 2}, 
                                     {"b", 5}, 
                                     {"x", 3} 
                                 };
            IDbParameters parameters = _mockery.CreateMock<IDbParameters>();

            foreach (var pair in collection)
            {
                Expect.Call(_converter(pair.Key)).Return(parameters);
                int toReturn = pair.Value;
                Expect.Call(_adoOperations.ExecuteNonQuery(CommandType.Text, _sql, (ICommandSetter) null))
                    .IgnoreArguments().Do(new Func<CommandType, string, ICommandSetter, int>(
                                              delegate(CommandType cmdType, string cmdText, ICommandSetter setter)
                                                  {
                                                      Assert.AreEqual(CommandType.Text, cmdType);
                                                      Assert.AreEqual(_sql, cmdText);
                                                      return toReturn;
                                                  }
                                              ));
            }

            _mockery.ReplayAll();
            int result = AdoOperationsExtension.ExecuteNonQuery(_adoOperations, CommandType.Text, _sql, collection.Keys, _converter);
            Assert.That(result, Is.EqualTo(10));
            _mockery.VerifyAll();
        }
    }

    [TestFixture(typeof(string)), TestFixture(typeof(int))]
    public class AdoOperationsExtensionTest<T>
    {
        private MockRepository _mockery;
        private IAdoOperations _adoOperations;
        private const string _sql = "fake sql statement";
        private IDataRecordOrdinalCache _ordinalCache;
        private int _rowsExpected;
        private IRowMapper<T> _rowMapper;
        private RowMapperDelegate<T> _rowMapperDelegate;
        readonly IList<T> _expectedList = new List<T>();

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _adoOperations = _mockery.CreateMock<IAdoOperations>();
            _ordinalCache = _mockery.CreateMock<IDataRecordOrdinalCache>();
            _rowMapper = _mockery.CreateMock<IRowMapper<T>>();
            _rowMapperDelegate = _mockery.CreateMock<RowMapperDelegate<T>>();
        }

        #region QueryWithRowMapper

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapper(CommandType commandType, int rowsExpected)
        {
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, bool>(
                              (cmdType, sql, extractor) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapper(
                commandType, _sql, _rowMapper, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapperWithCommandSetter(CommandType commandType, int rowsExpected)
        {
            var setter = _mockery.CreateMock<ICommandSetter>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null, setter))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, ICommandSetter, bool>(
                              (cmdType, sql, extractor, cmdSetter) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(cmdSetter, setter) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapper(
                commandType, _sql, _rowMapper, setter, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapperWithDbParameters(CommandType commandType, int rowsExpected)
        {
            var parameters = _mockery.CreateMock<IDbParameters>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null, parameters))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, IDbParameters, bool>(
                              (cmdType, sql, extractor, dbParameters) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(dbParameters, parameters) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapper(
                commandType, _sql, _rowMapper, parameters, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1, DbType.String, 0, "string value")]
        [TestCase(CommandType.StoredProcedure, 10, DbType.Single, 2, 33.33)]
        public void QueryWithRowMapperWithOneParameter(
            CommandType commandType, int rowsExpected, Enum dbType, int size, object value)
        {
            const string paramName = "paramName";
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(
                    CommandType.Text, _sql, null, paramName, dbType, size, value))
                .Return(_expectedList)
                .Callback(new OneParameterCall(
                              (cmdType, sql, extractor, theName, theType, theSize, theValue) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ReferenceEquals(theName, paramName) &&
                                  dbType == theType &&
                                  size == theSize &&
                                  value.Equals(theValue) &&
                                  ValidateExtendedRowMapperResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapper(
                commandType, _sql, _rowMapper, paramName, dbType, size, value, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        private bool ValidateExtendedRowMapperResultSetExtractor(IResultSetExtractor<IList<T>> extractor)
        {
            var extended = extractor as ExtendedRowMapperResultSetExtractor<T>;
            return extended != null &&
                   extended.RowsExpected == _rowsExpected &&
                   ReferenceEquals(extended.OrdinalCache, _ordinalCache) &&
                   ReferenceEquals(extended._rowMapper, _rowMapper);
        }

        #endregion        
        
        #region QueryWithRowMapperDelegate

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapperDelegate(CommandType commandType, int rowsExpected)
        {
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, bool>(
                              (cmdType, sql, extractor) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapperDelegate(
                commandType, _sql, _rowMapperDelegate, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapperDelegateWithCommandSetter(CommandType commandType, int rowsExpected)
        {
            var setter = _mockery.CreateMock<ICommandSetter>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null, setter))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, ICommandSetter, bool>(
                              (cmdType, sql, extractor, cmdSetter) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(cmdSetter, setter) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapperDelegate(
                commandType, _sql, _rowMapperDelegate, setter, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1)]
        [TestCase(CommandType.StoredProcedure, 10)]
        public void QueryWithRowMapperDelegateWithDbParameters(CommandType commandType, int rowsExpected)
        {
            var parameters = _mockery.CreateMock<IDbParameters>();
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(CommandType.Text, _sql, null, parameters))
                .Return(_expectedList)
                .Callback(new Func<CommandType, string, IResultSetExtractor<IList<T>>, IDbParameters, bool>(
                              (cmdType, sql, extractor, dbParameters) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(dbParameters, parameters) &&
                                  ReferenceEquals(sql, _sql) &&
                                  ValidateExtendedRowMapperDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapperDelegate(
                commandType, _sql, _rowMapperDelegate, parameters, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        [TestCase(CommandType.Text, 1, DbType.String, 0, "string value")]
        [TestCase(CommandType.StoredProcedure, 10, DbType.Single, 2, 33.33)]
        public void QueryWithRowMapperDelegateWithOneParameter(
            CommandType commandType, int rowsExpected, Enum dbType, int size, object value)
        {
            const string paramName = "paramName";
            Expect.Call(_adoOperations.QueryWithResultSetExtractor<IList<T>>(
                    CommandType.Text, _sql, null, paramName, dbType, size, value))
                .Return(_expectedList)
                .Callback(new OneParameterCall(
                              (cmdType, sql, extractor, theName, theType, theSize, theValue) =>
                                  cmdType == commandType &&
                                  ReferenceEquals(sql, _sql) &&
                                  ReferenceEquals(theName, paramName) &&
                                  dbType == theType &&
                                  size == theSize &&
                                  value.Equals(theValue) &&
                                  ValidateExtendedRowMapperDelegateResultSetExtractor(extractor)
                              ));
            _mockery.ReplayAll();
            _rowsExpected = rowsExpected;
            var result = _adoOperations.QueryWithRowMapperDelegate(
                commandType, _sql, _rowMapperDelegate, paramName, dbType, size, value, _ordinalCache, rowsExpected);
            Assert.That(result, Is.SameAs(_expectedList));
            _mockery.VerifyAll();
        }

        private bool ValidateExtendedRowMapperDelegateResultSetExtractor(IResultSetExtractor<IList<T>> extractor)
        {
            var extended = extractor as ExtendedRowMapperResultSetExtractor<T>;
            return extended != null &&
                   extended.RowsExpected == _rowsExpected &&
                   ReferenceEquals(extended.OrdinalCache, _ordinalCache) &&
                   ReferenceEquals(extended._rowMapperDelegate, _rowMapperDelegate);
        }

        #endregion
        
        private delegate bool OneParameterCall(CommandType cmdType, string cmdText, IResultSetExtractor<IList<T>> extractor, string paramName, Enum dbType, int size, object value);

    }
}