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
using NUnit.Framework;
using Rhino.Mocks;
using Spring.Data.Support;

namespace Spring.Extension.Tests.Data.Support
{
    /// <summary>
    /// Test cases for <see cref="DataRecordOrdinalCache"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class DataRecordOrdinalCacheTest
    {
        private MockRepository _mockery;
        private DataRecordOrdinalCache _testee;
        private IDataReader _dataReader;

        private static readonly string[] _fieldNames = new string[]{"lower_case", "UPPERCASE", "CamelCase"};

        [SetUp]
        public void SetUp()
        {
            _mockery = new MockRepository();
            _testee = new DataRecordOrdinalCache();
            _dataReader = _mockery.CreateMock<IDataReader>();

            Expect.Call(_dataReader.FieldCount).Return(_fieldNames.Length);
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                Expect.Call(_dataReader.GetName(i)).Return(_fieldNames[i]);
            }
        }

        [Test] public void InitChokesOnNullDataRecord()
        {
            var e = Assert.Throws<ArgumentNullException>(() => _testee.Init(null));
            Assert.That(e.Message.Contains("dataRecord"));
        }

        [Test] public void InitCachesOnFirstCall()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            _mockery.VerifyAll();
        }

        [Test] public void InitIgnoresSubsequentCall()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            _testee.Init(_dataReader); // second call
            _mockery.VerifyAll();
        }

        [Test] public void GetOridnalChokesOnNullName()
        {
            var e = Assert.Throws<ArgumentNullException>(() => _testee.GetOrdinal(null));
            Assert.That(e.Message.Contains("name"));
        }

        [Test] public void GetOridnalChokesBeforeInit()
        {
            var e = Assert.Throws<InvalidOperationException>(() => _testee.GetOrdinal("any"));
            Assert.That(e.Message.Contains("Init"));
        }

        [Test] public void GetOridnalChokesWhenNameDoesNotExist()
        {
            _mockery.ReplayAll();
            const string nameDoesNotExist = "nameDoesNotExist";
            _testee.Init(_dataReader);
            var e = Assert.Throws<IndexOutOfRangeException>(() => _testee.GetOrdinal(nameDoesNotExist));
            Assert.That(e.Message.Contains(nameDoesNotExist));
        }

        [Test] public void GetOrdinalMatchesExactName()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                Assert.That(_testee.GetOrdinal(_fieldNames[i]), Is.EqualTo(i));
            }
        }

        [Test] public void GetOrinalCaseInsensitive()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            Assert.That(_testee.GetOrdinal("LOWER_CASE"), Is.EqualTo(0));
            Assert.That(_testee.GetOrdinal("Lower_Case"), Is.EqualTo(0));
            Assert.That(_testee.GetOrdinal("uppercase"), Is.EqualTo(1));
            Assert.That(_testee.GetOrdinal("CAMELCASE"), Is.EqualTo(2));
            Assert.That(_testee.GetOrdinal("cAMELcASE"), Is.EqualTo(2));
        }
    }
}
