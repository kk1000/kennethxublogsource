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
    [TestFixture] public class AdoOperationsExtensionTest
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
}