using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Spring.Data.Support;

namespace Spring.Extension.Tests.Data.Support
{
    [TestFixture] public class DataReaderWrapperBaseTest
    {
        private MockRepository _mockery;
        private DataReaderWrapperBase _testee;
        private IDataReader _wrapped;

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _testee = new DataReaderWrapperBase();
            _wrapped = _mockery.CreateMock<IDataReader>();
            _testee.WrappedReader = _wrapped;
        }

        #region Properties
        [Test] public void WrappedReaderProperty()
        {
            Assert.AreSame(_wrapped, _testee.WrappedReader);
        }

        [Test] public void DepthProperty()
        {
            int depth = 8433; //arbitrary number
            Expect.Call(_wrapped.Depth).Return(depth);
            _mockery.ReplayAll();
            Assert.AreEqual(depth, _testee.Depth);
            _mockery.VerifyAll();
        }

        [Test] public void FieldCountProperty()
        {
            int count = 3884; //arbitrary number
            Expect.Call(_wrapped.FieldCount).Return(count);
            _mockery.ReplayAll();
            Assert.AreEqual(count, _testee.FieldCount);
            _mockery.VerifyAll();
        }

        [Test] public void IsClosedProperty()
        {
            Expect.Call(_wrapped.IsClosed).Return(false);
            Expect.Call(_wrapped.IsClosed).Return(true);
            _mockery.ReplayAll();
            Assert.IsFalse(_testee.IsClosed);
            Assert.IsTrue(_testee.IsClosed);
            _mockery.VerifyAll();
        }

        [Test] public void RecordsAffectedProperty()
        {
            int count = 95747; //arbitrary number
            Expect.Call(_wrapped.RecordsAffected).Return(count);
            _mockery.ReplayAll();
            Assert.AreEqual(count, _testee.RecordsAffected);
            _mockery.VerifyAll();
        }

        #endregion

        #region Indexers

        [Test] public void IndexerByPosition()
        {
            string v1 = "jfdskfjafdsa";
            int v2 = 4329432;
            Expect.Call(_wrapped[1]).Return(v1);
            Expect.Call(_wrapped[5]).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee[1]);
            Assert.AreEqual(v2, _testee[5]);
            _mockery.VerifyAll();
        }

        [Test] public void IndexerByName()
        {
            string v1 = "jfdskfjafdsa";
            int v2 = 4329432;
            Expect.Call(_wrapped["f1"]).Return(v1);
            Expect.Call(_wrapped["f2"]).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee["f1"]);
            Assert.AreEqual(v2, _testee["f2"]);
            _mockery.VerifyAll();
        }

        #endregion

        #region Value Getters

        [Test] public void GetBooleanMethod()
        {
            Expect.Call(_wrapped.GetBoolean(1)).Return(false);
            Expect.Call(_wrapped.GetBoolean(5)).Return(true);
            _mockery.ReplayAll();
            Assert.IsFalse(_testee.GetBoolean(1));
            Assert.IsTrue(_testee.GetBoolean(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetByteMethod()
        {
            Expect.Call(_wrapped.GetByte(1)).Return(34);
            Expect.Call(_wrapped.GetByte(5)).Return(55);
            _mockery.ReplayAll();
            Assert.AreEqual(34, _testee.GetByte(1));
            Assert.AreEqual(55, _testee.GetByte(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetBytesMethod()
        {
            byte[] buffer = new byte[] {3, 4, 5};
            Expect.Call(_wrapped.GetBytes(1, 1, buffer, 1, 1)).Return(34);
            Expect.Call(_wrapped.GetBytes(5, 5, buffer, 5, 5)).Return(55);
            _mockery.ReplayAll();
            Assert.AreEqual(34, _testee.GetBytes(1, 1, buffer, 1, 1));
            Assert.AreEqual(55, _testee.GetBytes(5, 5, buffer, 5, 5));
            _mockery.VerifyAll();
        }

        [Test] public void GetCharMethod()
        {
            Expect.Call(_wrapped.GetChar(1)).Return('A');
            Expect.Call(_wrapped.GetChar(5)).Return('x');
            _mockery.ReplayAll();
            Assert.AreEqual('A', _testee.GetChar(1));
            Assert.AreEqual('x', _testee.GetChar(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetCharsMethod()
        {
            char[] buffer = new char[] {'1'};
            Expect.Call(_wrapped.GetChars(1, 1, buffer, 1, 1)).Return(34);
            Expect.Call(_wrapped.GetChars(5, 5, buffer, 5, 5)).Return(55);
            _mockery.ReplayAll();
            Assert.AreEqual(34, _testee.GetChars(1, 1, buffer, 1, 1));
            Assert.AreEqual(55, _testee.GetChars(5, 5, buffer, 5, 5));
            _mockery.VerifyAll();
        }

        [Test] public void GetDateTimeMethod()
        {
            var d1 = new DateTime(1999, 9, 9);
            var d2 = new DateTime(2022, 3, 30);
            Expect.Call(_wrapped.GetDateTime(1)).Return(d1);
            Expect.Call(_wrapped.GetDateTime(5)).Return(d2);
            _mockery.ReplayAll();
            Assert.AreEqual(d1, _testee.GetDateTime(1));
            Assert.AreEqual(d2, _testee.GetDateTime(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetDecimalMethod()
        {
            var d1 = new decimal(1999.9);
            var d2 = new decimal(2022.30);
            Expect.Call(_wrapped.GetDecimal(1)).Return(d1);
            Expect.Call(_wrapped.GetDecimal(5)).Return(d2);
            _mockery.ReplayAll();
            Assert.AreEqual(d1, _testee.GetDecimal(1));
            Assert.AreEqual(d2, _testee.GetDecimal(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetDoubleMethod()
        {
            var d1 = 1999.9d;
            var d2 = 2022.30d;
            Expect.Call(_wrapped.GetDouble(1)).Return(d1);
            Expect.Call(_wrapped.GetDouble(5)).Return(d2);
            _mockery.ReplayAll();
            Assert.AreEqual(d1, _testee.GetDouble(1));
            Assert.AreEqual(d2, _testee.GetDouble(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetFloatMethod()
        {
            var d1 = 1999.9f;
            var d2 = 2022.30f;
            Expect.Call(_wrapped.GetFloat(1)).Return(d1);
            Expect.Call(_wrapped.GetFloat(5)).Return(d2);
            _mockery.ReplayAll();
            Assert.AreEqual(d1, _testee.GetFloat(1));
            Assert.AreEqual(d2, _testee.GetFloat(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetGuidMethod()
        {
            var d1 = new Guid();
            var d2 = new Guid();
            Expect.Call(_wrapped.GetGuid(1)).Return(d1);
            Expect.Call(_wrapped.GetGuid(5)).Return(d2);
            _mockery.ReplayAll();
            Assert.AreEqual(d1, _testee.GetGuid(1));
            Assert.AreEqual(d2, _testee.GetGuid(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetInt16Method()
        {
            short v1 = 199;
            short v2 = 202;
            Expect.Call(_wrapped.GetInt16(1)).Return(v1);
            Expect.Call(_wrapped.GetInt16(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetInt16(1));
            Assert.AreEqual(v2, _testee.GetInt16(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetInt32Method()
        {
            int v1 = 94748;
            int v2 = -4938;
            Expect.Call(_wrapped.GetInt32(1)).Return(v1);
            Expect.Call(_wrapped.GetInt32(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetInt32(1));
            Assert.AreEqual(v2, _testee.GetInt32(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetInt64Method()
        {
            long v1 = 239231437432;
            long v2 = -93879984;
            Expect.Call(_wrapped.GetInt64(1)).Return(v1);
            Expect.Call(_wrapped.GetInt64(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetInt64(1));
            Assert.AreEqual(v2, _testee.GetInt64(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetStringMethod()
        {
            string v1 = "jfdskfjafdsa";
            string v2 = "";
            Expect.Call(_wrapped.GetString(1)).Return(v1);
            Expect.Call(_wrapped.GetString(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetString(1));
            Assert.AreEqual(v2, _testee.GetString(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetValueMethod()
        {
            string v1 = "jfdskfjafdsa";
            int v2 = 4329432;
            Expect.Call(_wrapped.GetValue(1)).Return(v1);
            Expect.Call(_wrapped.GetValue(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetValue(1));
            Assert.AreEqual(v2, _testee.GetValue(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetValuesMethod()
        {
            object [] v1 = new object[] { "jfdskfjafdsa", 4329432 };
            Expect.Call(_wrapped.GetValues(v1)).Return(2);
            _mockery.ReplayAll();
            Assert.AreEqual(2, _testee.GetValues(v1));
            _mockery.VerifyAll();
        }

        #endregion

        #region Methods

        [Test] public void GetDataMethod()
        {
            IDataReader r = _mockery.Stub<IDataReader>();
            Expect.Call(_wrapped.GetData(5)).Return(r);
            _mockery.ReplayAll();
            Assert.AreSame(r, _testee.GetData(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetDataTypeNameMethod()
        {
            string v1 = "Int32";
            string v2 = "String";
            Expect.Call(_wrapped.GetDataTypeName(1)).Return(v1);
            Expect.Call(_wrapped.GetDataTypeName(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetDataTypeName(1));
            Assert.AreEqual(v2, _testee.GetDataTypeName(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetFieldTypeMethod()
        {
            Type v1 = typeof(int);
            Type v2 = typeof(string);
            Expect.Call(_wrapped.GetFieldType(1)).Return(v1);
            Expect.Call(_wrapped.GetFieldType(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetFieldType(1));
            Assert.AreEqual(v2, _testee.GetFieldType(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetNameMethod()
        {
            string v1 = "field1";
            string v2 = "fieldx";
            Expect.Call(_wrapped.GetName(1)).Return(v1);
            Expect.Call(_wrapped.GetName(5)).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetName(1));
            Assert.AreEqual(v2, _testee.GetName(5));
            _mockery.VerifyAll();
        }

        [Test] public void GetOrdinalMethod()
        {
            int v1 = 1;
            int v2 = 6;
            Expect.Call(_wrapped.GetOrdinal("f1")).Return(v1);
            Expect.Call(_wrapped.GetOrdinal("fx")).Return(v2);
            _mockery.ReplayAll();
            Assert.AreEqual(v1, _testee.GetOrdinal("f1"));
            Assert.AreEqual(v2, _testee.GetOrdinal("fx"));
            _mockery.VerifyAll();
        }

        [Test] public void GetSchemaTableMethod()
        {
            DataTable r = _mockery.Stub<DataTable>();
            Expect.Call(_wrapped.GetSchemaTable()).Return(r);
            _mockery.ReplayAll();
            Assert.AreSame(r, _testee.GetSchemaTable());
            _mockery.VerifyAll();
        }

        [Test] public void IsDBNullMethod()
        {
            Expect.Call(_wrapped.IsDBNull(1)).Return(false);
            Expect.Call(_wrapped.IsDBNull(5)).Return(true);
            _mockery.ReplayAll();
            Assert.IsFalse(_testee.IsDBNull(1));
            Assert.IsTrue(_testee.IsDBNull(5));
            _mockery.VerifyAll();
        }

        [Test] public void NextResultMethod()
        {
            Expect.Call(_wrapped.NextResult()).Return(false);
            Expect.Call(_wrapped.NextResult()).Return(true);
            _mockery.ReplayAll();
            Assert.IsFalse(_testee.NextResult());
            Assert.IsTrue(_testee.NextResult());
            _mockery.VerifyAll();
        }

        [Test] public void ReadMethod()
        {
            Expect.Call(_wrapped.Read()).Return(false);
            Expect.Call(_wrapped.Read()).Return(true);
            _mockery.ReplayAll();
            Assert.IsFalse(_testee.Read());
            Assert.IsTrue(_testee.Read());
            _mockery.VerifyAll();
        }

        [Test] public void CloseMethod()
        {
            _wrapped.Close();
            _mockery.ReplayAll();
            _testee.Close();
            _mockery.VerifyAll();
        }

        [Test] public void DisposeMethod()
        {
            _wrapped.Dispose();
            _mockery.ReplayAll();
            _testee.Dispose();
            _mockery.VerifyAll();
        }

        #endregion
    }
}