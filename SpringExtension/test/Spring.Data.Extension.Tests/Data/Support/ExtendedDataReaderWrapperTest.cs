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
using NUnit.Framework;
using Rhino.Mocks;

namespace Spring.Data.Support
{
    /// <summary>
    /// Test cases for <see cref="ExtendedDataReaderWrapper"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class ExtendedDataReaderWrapperTest
    {
        private MockRepository _mockery;
        private ExtendedDataReaderWrapper _testee;
        private IDataReader _wrapped;
        private IDataRecordOrdinalCache _cache;
        private const string _fieldName = "field1";

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _testee = new ExtendedDataReaderWrapper();
            _wrapped = _mockery.CreateMock<IDataReader>();
            _cache = _mockery.CreateMock<IDataRecordOrdinalCache>();
        }

        [Test] public void GetOrdinalCallsWrapperWithoutCache()
        {
            Expect.Call(_wrapped.GetOrdinal(_fieldName)).Return(3);
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            Assert.AreEqual(3, _testee.GetOrdinal(_fieldName));
            Assert.That(_testee.WrappedReader, Is.EqualTo(_wrapped));
            _mockery.VerifyAll();
        }

        [Test] public void GetOrdinalCallsCacheIfPresent()
        {
            Expect.Call(_cache.GetOrdinal(_fieldName)).Return(3);
            _mockery.ReplayAll();
            _testee.OrdinalCache = _cache;
            Assert.AreEqual(3, _testee.GetOrdinal(_fieldName));
            _mockery.VerifyAll();
        }

        [Test] public void GetInnerMostReaderSingleLayer()
        {
            _testee.WrappedReader = _wrapped;
            _mockery.ReplayAll();
            Assert.That(_testee.GetInnerMostReader(), Is.EqualTo(_wrapped));
        }

        [Test] public void GetInnerMostReaderMultiLayer()
        {
            var wrapper1 = new ExtendedDataReaderWrapper{WrappedReader = _wrapped};
            var wrapper2 = new NullMappingDataReader(wrapper1);
            _testee.WrappedReader = wrapper2;
            _mockery.ReplayAll();
            Assert.That(_testee.GetInnerMostReader(), Is.EqualTo(_wrapped));
        }

        [Test] public void InitOrdinalCacheWhenWrappedReaderIsSetBefore()
        {
            _cache.Init(_wrapped);
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            _testee.OrdinalCache = _cache;
            Assert.That(_testee.OrdinalCache, Is.EqualTo(_cache));
            _mockery.VerifyAll();
        }

        [Test] public void InitOrdinalCacheWhenWrappedReaderIsSetAfter()
        {
            _cache.Init(_wrapped);
            _mockery.ReplayAll();
            _testee.OrdinalCache = _cache;
            _testee.WrappedReader = _wrapped;
            Assert.That(_testee.OrdinalCache, Is.EqualTo(_cache));
            _mockery.VerifyAll();
        }

        [Test] public void RowsExpectedSetsFineWhenNoWrapped()
        {
            const int rowsExpected = 22;
            _testee.RowsExpected = rowsExpected;
            Assert.That(_testee.RowsExpected, Is.EqualTo(rowsExpected));
        }

        [Test] public void RowsExpectedSetsFineWhenWrappedIsNotExtended()
        {
            const int rowsExpected = 34;
            _mockery.ReplayAll();
            _testee.WrappedReader = _wrapped;
            _testee.RowsExpected = rowsExpected;
            Assert.That(_testee.RowsExpected, Is.EqualTo(rowsExpected));
            _mockery.VerifyAll();
        }

        [Test] public void RowsExpectedSetsWrappedWhenItIsAlsoExtended()
        {
            const int rowsExptected = 213;
            var wrapped = _mockery.CreateMock<ExtendedDataReaderWrapper>();
            wrapped.RowsExpected = 0;
            wrapped.RowsExpected = rowsExptected;
            _mockery.ReplayAll();
            _testee.WrappedReader = wrapped;
            _testee.RowsExpected = rowsExptected;
            Assert.That(_testee.RowsExpected, Is.EqualTo(rowsExptected));
            _mockery.VerifyAll();
        }

        [Test] public void PropagateRowsExpectedWhenSetWrappedToExtendedWrapper()
        {
            const int rowsExptected = 324;
            var wrapped = _mockery.CreateMock<ExtendedDataReaderWrapper>();
            wrapped.RowsExpected = rowsExptected;
            _mockery.ReplayAll();
            _testee.RowsExpected = rowsExptected;
            _testee.WrappedReader = wrapped;
            Assert.That(_testee.RowsExpected, Is.EqualTo(rowsExptected));
            _mockery.VerifyAll();
        }
    }
}