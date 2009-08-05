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

namespace Spring.Data.Support
{
    /// <summary>
    /// Test cases for <see cref="ExtendedRowCallbackResultSetExtractor"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class ExtendedRowCallbackResultSetExtractorTest
    {
        private MockRepository _mockery;
        private IRowCallback _rowMapper;
        private RowCallbackDelegate _rowMapperDelegate;
        private IDataReader _dataReader;

        [SetUp]
        public void SetUp()
        {
            _mockery = new MockRepository();
            _rowMapper = _mockery.StrictMock<IRowCallback>();
            _rowMapperDelegate = _mockery.StrictMock<RowCallbackDelegate>();
            _dataReader = _mockery.StrictMock<IDataReader>();
        }

        [Test]
        public void ChokesOnConstructingWithNullRowMapper()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExtendedRowCallbackResultSetExtractor((IRowCallback)null));
        }

        [Test]
        public void ChokesOnConstructingWithNullRowMapperDelegate()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExtendedRowCallbackResultSetExtractor((RowCallbackDelegate)null));
        }

        [Test] public void ExtractDataWithRowMapper([Values(0, 2, 10)] int n)
        {
            for (int i = 0; i < n; i++)
            {
                Expect.Call(_dataReader.Read()).Return(true);
                _rowMapper.ProcessRow(_dataReader);
            }
            Expect.Call(_dataReader.Read()).Return(false);
            _mockery.ReplayAll();
            var testee = new ExtendedRowCallbackResultSetExtractor(_rowMapper);
            testee.ExtractData(_dataReader);
            _mockery.VerifyAll();

        }

        [Test] public void ExtractDataWithRowMapperDelegate([Values(0, 2, 10)] int n)
        {
            for (int i = 0; i < n; i++)
            {
                Expect.Call(_dataReader.Read()).Return(true);
                _rowMapperDelegate(_dataReader);
            }
            Expect.Call(_dataReader.Read()).Return(false);
            _mockery.ReplayAll();
            var testee = new ExtendedRowCallbackResultSetExtractor(_rowMapperDelegate);
            testee.ExtractData(_dataReader);
            _mockery.VerifyAll();

        }

        [Test] public void ProtectedConstructorSetsItselfWhenAlsoRowCallback()
        {
            var testee = _mockery.PartialMultiMock<ExtendedRowCallbackResultSetExtractor>(
                typeof (IRowCallback));
            _mockery.ReplayAll();
            Assert.That(testee._rowCallback, Is.SameAs(testee));
            _mockery.VerifyAll();
        }
    }
}
