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
    /// Test cases for <see cref="DataReaderExtender"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class DataReaderExtenderTest
    {
        private MockRepository _mockery;
        private IDataReader _dataReader;
        private IDataRecordOrdinalCache _ordinalCache;
        private DataReaderExtender testee;

        [SetUp]
        public void SetUp()
        {
            testee = new DataReaderExtender();
            _mockery = new MockRepository();
            _dataReader = _mockery.StrictMock<IDataReader>();
            _ordinalCache = _mockery.StrictMock<IDataRecordOrdinalCache>();
        }

        [Test]
        public void WrapDataReaderReturnsOriginalWhenNoCacheNoRowsExpected()
        {
            var reader = testee.ExtendDataReader(_dataReader);
            Assert.That(reader, Is.SameAs(_dataReader));
        }

        [TestCase(0), TestCase(10), TestCase(-3)]
        public void WrapDataReaderReturnsExtendedWhenCachePresent(int rowsExpected)
        {
            _ordinalCache.Init(_dataReader);
            _mockery.ReplayAll();
            testee.OrdinalCache = _ordinalCache;
            testee.RowsExpected = rowsExpected;
            var reader = testee.ExtendDataReader(_dataReader);
            Assert.That(reader, Is.Not.Null);
            Assert.That(reader, Is.InstanceOf(typeof(ExtendedDataReaderWrapper)));
            var wrapper = (ExtendedDataReaderWrapper)reader;
            Assert.That(wrapper.OrdinalCache, Is.SameAs(_ordinalCache));
            Assert.That(testee.OrdinalCache, Is.SameAs(_ordinalCache));
            Assert.That(wrapper.RowsExpected, Is.EqualTo(rowsExpected > 0 ? rowsExpected : 0));
            Assert.That(testee.RowsExpected, Is.EqualTo(rowsExpected));
            _mockery.VerifyAll();
        }

        [TestCase(0), TestCase(10), TestCase(-3)]
        public void WrapDataReaderReturnsOriginalWhenAlreadyExtended(int rowsExpected)
        {
            var extendedReader = new ExtendedDataReaderWrapper();
            _ordinalCache.Init(_dataReader);
            _mockery.ReplayAll();
            extendedReader.WrappedReader = _dataReader;
            testee.OrdinalCache = _ordinalCache;
            testee.RowsExpected = rowsExpected;
            var reader = testee.ExtendDataReader(extendedReader);
            Assert.That(reader, Is.SameAs(extendedReader));
            Assert.That(extendedReader.OrdinalCache, Is.SameAs(_ordinalCache));
            Assert.That(testee.OrdinalCache, Is.SameAs(_ordinalCache));
            Assert.That(extendedReader.RowsExpected, Is.EqualTo(rowsExpected > 0 ? rowsExpected : 0));
            Assert.That(testee.RowsExpected, Is.EqualTo(rowsExpected));
            _mockery.VerifyAll();
        }
    }
}
