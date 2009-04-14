using System;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Spring.Data.Support;

namespace Spring.Extension.Tests.Data.Support
{
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
            TestHelper.AssertException<ArgumentNullException>(
                () => _testee.Init(null),
                MessageMatch.Contains,
                "dataRecord"
                );
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

        [Test] public void GetOridnalChokesBeforeInit()
        {
            TestHelper.AssertException<InvalidOperationException>(
                ()=>_testee.GetOrdinal("any"));
        }

        [Test] public void GetOridnalChokesWhenNameDoesNotExist()
        {
            _mockery.ReplayAll();
            const string nameDoesNotExist = "nameDoesNotExist";
            _testee.Init(_dataReader);
            TestHelper.AssertException<IndexOutOfRangeException>(
                ()=>_testee.GetOrdinal(nameDoesNotExist),
                MessageMatch.Contains,
                nameDoesNotExist);
        }

        [Test] public void GetOrdinalMatchesExactName()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                Assert.That(_testee.GetOrdinal(_fieldNames[i]), Iz.EqualTo(i));
            }
        }

        [Test] public void GetOrinalCaseInsensitive()
        {
            _mockery.ReplayAll();
            _testee.Init(_dataReader);
            Assert.That(_testee.GetOrdinal("LOWER_CASE"), Iz.EqualTo(0));
            Assert.That(_testee.GetOrdinal("Lower_Case"), Iz.EqualTo(0));
            Assert.That(_testee.GetOrdinal("uppercase"), Iz.EqualTo(1));
            Assert.That(_testee.GetOrdinal("CAMELCASE"), Iz.EqualTo(2));
            Assert.That(_testee.GetOrdinal("cAMELcASE"), Iz.EqualTo(2));
        }
    }
}
