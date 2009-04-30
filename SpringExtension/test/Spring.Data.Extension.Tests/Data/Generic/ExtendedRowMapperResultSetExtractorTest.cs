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
using Rhino.Mocks;

namespace Spring.Data.Generic
{
    /// <summary>
    /// Test cases for <see cref="ExtendedRowMapperResultSetExtractor{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <author>Kenneth Xu</author>
    [TestFixture(typeof(string))]
    [TestFixture(typeof(int))]
    public class ExtendedRowMapperResultSetExtractorTest<T>
    {
        private MockRepository _mockery;
	    private IRowMapper<T> _rowMapper;
        private RowMapperDelegate<T> _rowMapperDelegate;
        private IDataReader _dataReader;

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _rowMapper = _mockery.CreateMock<IRowMapper<T>>();
            _rowMapperDelegate = _mockery.CreateMock<RowMapperDelegate<T>>();
            _dataReader = _mockery.CreateMock<IDataReader>();
        }

        [Test] public void ChokesOnConstructingWithNullRowMapper()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExtendedRowMapperResultSetExtractor<T>((IRowMapper<T>)null));
        }

        [Test] public void ChokesOnConstructingWithNullRowMapperDelegate()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExtendedRowMapperResultSetExtractor<T>((RowMapperDelegate<T>)null));
        }

        [Test] public void ExtractDataWithRowMapper([Values(0, 2, 10)] int n)
        {
            var expected = new List<T>(n);
            for(int i=0; i<n; i++)
            {
                T value = (T) Convert.ChangeType(i, typeof (T));
                Expect.Call(_dataReader.Read()).Return(true);
                Expect.Call(_rowMapper.MapRow(_dataReader, i)).Return(value);
                expected.Add(value);
            }
            Expect.Call(_dataReader.Read()).Return(false);
            _mockery.ReplayAll();
            var testee = new ExtendedRowMapperResultSetExtractor<T>(_rowMapper);
            IList<T> result = testee.ExtractData(_dataReader);
            CollectionAssert.AreEqual(expected, result);
            _mockery.VerifyAll();

        }

        [Test] public void ExtractDataWithRowMapperDelegate([Values(0, 2, 10)] int n)
        {
            var expected = new List<T>(n);
            for(int i=0; i<n; i++)
            {
                T value = (T) Convert.ChangeType(i, typeof (T));
                Expect.Call(_dataReader.Read()).Return(true);
                Expect.Call(_rowMapperDelegate(_dataReader, i)).Return(value);
                expected.Add(value);
            }
            Expect.Call(_dataReader.Read()).Return(false);
            _mockery.ReplayAll();
            var testee = new ExtendedRowMapperResultSetExtractor<T>(_rowMapperDelegate);
            IList<T> result = testee.ExtractData(_dataReader);
            CollectionAssert.AreEqual(expected, result);
            _mockery.VerifyAll();

        }

        [Test] public void ProtectedConstructorSetsItselfWhenAlsoRowCallback()
        {
            var testee = _mockery.PartialMultiMock<ExtendedRowMapperResultSetExtractor<T>>(
                typeof (IRowMapper<T>));
            _mockery.ReplayAll();
            Assert.That(testee._rowMapper, Is.SameAs(testee));
            _mockery.VerifyAll();
        }
    }
}
