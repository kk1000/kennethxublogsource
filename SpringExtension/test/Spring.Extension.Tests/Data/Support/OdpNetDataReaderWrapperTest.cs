using System;
using System.Data;
using NUnit.Framework;
using Oracle.DataAccess.Client;
using Rhino.Mocks;
using Spring.Data.Support;

namespace Spring.Extension.Tests.Data.Support
{
    [TestFixture] public class OdpNetDataReaderWrapperTest
    {
        private MockRepository _mockery;
        private DataReaderWrapperBase _testee;
        private IDataReader _wrapped;

        [SetUp]
        public void SetUp()
        {
            _mockery = new MockRepository();
            _testee = new OdpNetDataReaderWrapper();
            _wrapped = _mockery.CreateMock<IDataReader>();
            _testee.WrappedReader = _wrapped;
        }

        [Test] public void GetCharMethod()
        {
            Expect.Call(_wrapped.GetString(1)).Return("A");
            Expect.Call(_wrapped.GetString(5)).Return("Error");
            _mockery.ReplayAll();
            Assert.AreEqual('A', _testee.GetChar(1));
            TestHelper.AssertException<Exception>(() => _testee.GetChar(5));
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
            TestHelper.AssertException<InvalidCastException>(()=>_testee.GetBoolean(15));
            _mockery.VerifyAll();
        }
    }
}