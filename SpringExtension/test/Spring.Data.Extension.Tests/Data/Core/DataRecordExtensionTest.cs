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

namespace Spring.Data.Core
{
    /// <summary>
    /// Test cases for <see cref="DataRecordExtension"/>
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class DataRecordExtensionTest
    {
        private MockRepository _mockery;
        private IDataReader _dataReader;

        private const string _fieldName1 = "FieldName102";
        private const int _fieldIndex1 = 102;
        private const string _fieldName2 = "FieldName104";
        private const int _fieldIndex2 = 104;

        [SetUp] public void SetUp()
        {
            _mockery = new MockRepository();
            _dataReader = _mockery.StrictMock<IDataReader>();
        }

        #region GetBoolean

        private const bool _valueOfGetBoolean = true;
        private const bool _defaultOfGetBoolean = false;
        private const bool _defaultBoolean = false;

        [Test] public void GetBooleanChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, "True", "False"));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, 'Y', 'N'));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, "True"));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, 'Y'));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", "True", "False"));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", 'Y', 'N'));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", "True"));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", 'Y'));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, ""));
        }

        [Test] public void GetBooleanChokesOnNullTrueValue()
        {
            SetupResult.For(_dataReader.GetString(0)).IgnoreArguments().Return("any");
            SetupResult.For(_dataReader.GetChar(0)).IgnoreArguments().Return('A');
            SetupResult.For(_dataReader.GetOrdinal("")).IgnoreArguments().Return(0);

            _mockery.ReplayAll();
            ExpectArguementNullExceptionForTrueValue(() => DataRecordExtension.GetBoolean(_dataReader, 0, null, "False"));
            //Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, 'Y', 'N'));
            ExpectArguementNullExceptionForTrueValue(() => DataRecordExtension.GetBoolean(_dataReader, 0, null));
            //Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, 0, 'Y'));
            ExpectArguementNullExceptionForTrueValue(() => DataRecordExtension.GetBoolean(_dataReader, "", null, "False"));
            //Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", 'Y', 'N'));
            ExpectArguementNullExceptionForTrueValue(() => DataRecordExtension.GetBoolean(_dataReader, "", null));
            //Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, "", 'Y'));
            //Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetBoolean(null, ""));
        }

        [Test] public void GetBooleanByIndex()
        {
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, 1, "True"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 2, "True"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 3, "True"));
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, 1, "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 2, "True", "False"));
            ArgumentException e;
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.GetBoolean(_dataReader, 3, "True", "False"));
            StringAssert.Contains("True", e.Message);
            StringAssert.Contains("False", e.Message);
            StringAssert.Contains("Bad", e.Message);
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, 1, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 2, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 3, 'Y'));
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, 1, 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, 2, 'Y', 'N'));
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.GetBoolean(_dataReader, 3, 'Y', 'N'));
            StringAssert.Contains("Y", e.Message);
            StringAssert.Contains("N", e.Message);
            StringAssert.Contains("X", e.Message);
            _mockery.VerifyAll();
        }

        [Test] public void GetBooleanByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetBoolean(_fieldIndex1)).Return(_valueOfGetBoolean);
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal("1")).Return(1).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("2")).Return(2).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("3")).Return(3).Repeat.Any();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.GetBoolean(_dataReader, _fieldName1));
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, "1", "True"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "2", "True"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "3", "True"));
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, "1", "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "2", "True", "False"));
            ArgumentException e;
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.GetBoolean(_dataReader, "3", "True", "False"));
            StringAssert.Contains("True", e.Message);
            StringAssert.Contains("False", e.Message);
            StringAssert.Contains("Bad", e.Message);
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, "1", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "2", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "3", 'Y'));
            Assert.AreEqual(true, DataRecordExtension.GetBoolean(_dataReader, "1", 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.GetBoolean(_dataReader, "2", 'Y', 'N'));
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.GetBoolean(_dataReader, "3", 'Y', 'N'));
            StringAssert.Contains("Y", e.Message);
            StringAssert.Contains("N", e.Message);
            StringAssert.Contains("X", e.Message);
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetBooleanChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetBoolean(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetBoolean(null, 0, _defaultOfGetBoolean));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetBoolean(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetBoolean(null, "", _defaultOfGetBoolean));
        }

        [Test] public void SafeGetBooleanByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetBoolean(_fieldIndex1)).Return(_valueOfGetBoolean).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();

            Expect.Call(_dataReader.IsDBNull(1)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(2)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(3)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(4)).Return(true).Repeat.Times(4);
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            _mockery.ReplayAll();

            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldIndex1, _defaultOfGetBoolean));
            Assert.AreEqual(_defaultOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldIndex2, _defaultOfGetBoolean));

            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, 1, "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 2, "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 3, "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 4, "True"));
            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, 1, "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 2, "True", "False"));
            ArgumentException e;
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.SafeGetBoolean(_dataReader, 3, "True", "False"));
            StringAssert.Contains("True", e.Message);
            StringAssert.Contains("False", e.Message);
            StringAssert.Contains("Bad", e.Message);
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 4, "True", "False"));

            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, 1, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 2, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 3, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 4, 'Y'));
            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, 1, 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 2, 'Y', 'N'));
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.SafeGetBoolean(_dataReader, 3, 'Y', 'N'));
            StringAssert.Contains("Y", e.Message);
            StringAssert.Contains("N", e.Message);
            StringAssert.Contains("X", e.Message);
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 4, 'Y', 'N'));

            _mockery.VerifyAll();
        }

        [Test] public void SafeGetBooleanByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetBoolean(_fieldIndex1)).Return(_valueOfGetBoolean).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();

            Expect.Call(_dataReader.IsDBNull(1)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(2)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(3)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(4)).Return(true).Repeat.Times(4);
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal("1")).Return(1).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("2")).Return(2).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("3")).Return(3).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("4")).Return(4).Repeat.Any();
            _mockery.ReplayAll();

            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldName1, _defaultOfGetBoolean));
            Assert.AreEqual(_defaultOfGetBoolean, DataRecordExtension.SafeGetBoolean(_dataReader, _fieldName2, _defaultOfGetBoolean));

            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, "1", "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "2", "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "3", "True"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "4", "True"));
            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, "1", "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "2", "True", "False"));
            ArgumentException e;
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.SafeGetBoolean(_dataReader, "3", "True", "False"));
            StringAssert.Contains("True", e.Message);
            StringAssert.Contains("False", e.Message);
            StringAssert.Contains("Bad", e.Message);
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "4", "True", "False"));

            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, "1", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "2", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "3", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "4", 'Y'));
            Assert.AreEqual(true, DataRecordExtension.SafeGetBoolean(_dataReader, "1", 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "2", 'Y', 'N'));
            e = Assert.Throws<ArgumentException>(() => DataRecordExtension.SafeGetBoolean(_dataReader, "3", 'Y', 'N'));
            StringAssert.Contains("Y", e.Message);
            StringAssert.Contains("N", e.Message);
            StringAssert.Contains("X", e.Message);
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, "4", 'Y', 'N'));

            _mockery.VerifyAll();
        }

        [Test] public void GetNullableBooleanChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableBoolean(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableBoolean(null, ""));
        }

        [Test] public void GetNullableBooleanByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetBoolean(_fieldIndex1)).Return(_valueOfGetBoolean);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);

            Expect.Call(_dataReader.IsDBNull(1)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(2)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(3)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(4)).Return(true).Repeat.Times(4);
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            _mockery.ReplayAll();

            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.GetNullableBoolean(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, _fieldIndex2));

            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, 1, "True"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, 2, "True"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, 3, "True"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 4, "True"));
            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, 1, "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, 2, "True", "False"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 3, "True", "False"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 4, "True", "False"));

            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, 1, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, 2, 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, 3, 'Y'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 4, 'Y'));
            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, 1, 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.SafeGetBoolean(_dataReader, 2, 'Y', 'N'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 3, 'Y', 'N'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, 4, 'Y', 'N'));

            _mockery.VerifyAll();
        }

        [Test] public void GetNullableBooleanByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetBoolean(_fieldIndex1)).Return(_valueOfGetBoolean);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);

            Expect.Call(_dataReader.IsDBNull(1)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(2)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(3)).Return(false).Repeat.Times(4);
            Expect.Call(_dataReader.IsDBNull(4)).Return(true).Repeat.Times(4);
            Expect.Call(_dataReader.GetString(1)).Return("True").Repeat.Twice();
            Expect.Call(_dataReader.GetString(2)).Return("False").Repeat.Twice();
            Expect.Call(_dataReader.GetString(3)).Return("Bad").Repeat.Twice();
            Expect.Call(_dataReader.GetChar(1)).Return('Y').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(2)).Return('N').Repeat.Twice();
            Expect.Call(_dataReader.GetChar(3)).Return('X').Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal("1")).Return(1).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("2")).Return(2).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("3")).Return(3).Repeat.Any();
            Expect.Call(_dataReader.GetOrdinal("4")).Return(4).Repeat.Any();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetBoolean, DataRecordExtension.GetNullableBoolean(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, _fieldName2));

            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, "1", "True"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "2", "True"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "3", "True"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "4", "True"));
            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, "1", "True", "False"));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "2", "True", "False"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "3", "True", "False"));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "4", "True", "False"));

            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, "1", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "2", 'Y'));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "3", 'Y'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "4", 'Y'));
            Assert.AreEqual(true, DataRecordExtension.GetNullableBoolean(_dataReader, "1", 'Y', 'N'));
            Assert.AreEqual(false, DataRecordExtension.GetNullableBoolean(_dataReader, "2", 'Y', 'N'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "3", 'Y', 'N'));
            Assert.AreEqual(null, DataRecordExtension.GetNullableBoolean(_dataReader, "4", 'Y', 'N'));

            _mockery.VerifyAll();
        }

        #endregion

        #region GetByte

        private const byte _valueOfGetByte = 8;
        private const byte _defaultOfGetByte = 100;
        private const byte _defaultByte = 0;

        [Test] public void GetByteChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetByte(null, ""));
        }

        [Test] public void GetByteByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetByte(_fieldIndex1)).Return(_valueOfGetByte);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.GetByte(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetByteChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetByte(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetByte(null, 0, _defaultOfGetByte));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetByte(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetByte(null, "", _defaultOfGetByte));
        }

        [Test] public void SafeGetByteByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetByte(_fieldIndex1)).Return(_valueOfGetByte).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldIndex1, _defaultOfGetByte));
            Assert.AreEqual(_defaultOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldIndex2, _defaultOfGetByte));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetByteByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetByte(_fieldIndex1)).Return(_valueOfGetByte).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldName1, _defaultOfGetByte));
            Assert.AreEqual(_defaultOfGetByte, DataRecordExtension.SafeGetByte(_dataReader, _fieldName2, _defaultOfGetByte));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableByteChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableByte(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableByte(null, ""));
        }

        [Test] public void GetNullableByteByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetByte(_fieldIndex1)).Return(_valueOfGetByte);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.GetNullableByte(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableByte(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableByteByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetByte(_fieldIndex1)).Return(_valueOfGetByte);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetByte, DataRecordExtension.GetNullableByte(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableByte(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetChar

        private const char _valueOfGetChar = 'A';
        private const char _defaultOfGetChar = 'X';
        private const char _defaultChar = '\0';

        [Test] public void GetCharChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetChar(null, ""));
        }

        [Test] public void GetCharByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetChar(_fieldIndex1)).Return(_valueOfGetChar);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.GetChar(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetCharChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetChar(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetChar(null, 0, _defaultOfGetChar));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetChar(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetChar(null, "", _defaultOfGetChar));
        }

        [Test] public void SafeGetCharByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetChar(_fieldIndex1)).Return(_valueOfGetChar).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldIndex1, _defaultOfGetChar));
            Assert.AreEqual(_defaultOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldIndex2, _defaultOfGetChar));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetCharByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetChar(_fieldIndex1)).Return(_valueOfGetChar).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldName1, _defaultOfGetChar));
            Assert.AreEqual(_defaultOfGetChar, DataRecordExtension.SafeGetChar(_dataReader, _fieldName2, _defaultOfGetChar));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableCharChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableChar(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableChar(null, ""));
        }

        [Test] public void GetNullableCharByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetChar(_fieldIndex1)).Return(_valueOfGetChar);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.GetNullableChar(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableChar(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableCharByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetChar(_fieldIndex1)).Return(_valueOfGetChar);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetChar, DataRecordExtension.GetNullableChar(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableChar(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetDateTime

        private readonly DateTime _valueOfGetDateTime = DateTime.Now;
        private readonly DateTime _defaultOfGetDateTime = new DateTime(2000,1,1);
        private readonly DateTime _defaultDateTime = default(DateTime);

        [Test] public void GetDateTimeChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetDateTime(null, ""));
        }

        [Test] public void GetDateTimeByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetDateTime(_fieldIndex1)).Return(_valueOfGetDateTime);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.GetDateTime(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDateTimeChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDateTime(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDateTime(null, 0, _defaultOfGetDateTime));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDateTime(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDateTime(null, "", _defaultOfGetDateTime));
        }

        [Test] public void SafeGetDateTimeByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDateTime(_fieldIndex1)).Return(_valueOfGetDateTime).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldIndex1, _defaultOfGetDateTime));
            Assert.AreEqual(_defaultOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldIndex2, _defaultOfGetDateTime));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDateTimeByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDateTime(_fieldIndex1)).Return(_valueOfGetDateTime).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldName1, _defaultOfGetDateTime));
            Assert.AreEqual(_defaultOfGetDateTime, DataRecordExtension.SafeGetDateTime(_dataReader, _fieldName2, _defaultOfGetDateTime));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDateTimeChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDateTime(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDateTime(null, ""));
        }

        [Test] public void GetNullableDateTimeByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDateTime(_fieldIndex1)).Return(_valueOfGetDateTime);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.GetNullableDateTime(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDateTime(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDateTimeByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDateTime(_fieldIndex1)).Return(_valueOfGetDateTime);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDateTime, DataRecordExtension.GetNullableDateTime(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDateTime(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetDecimal

        private const decimal _valueOfGetDecimal = 8;
        private const decimal _defaultOfGetDecimal = 100;
        private const decimal _defaultDecimal = 0;

        [Test] public void GetDecimalChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetDecimal(null, ""));
        }

        [Test] public void GetDecimalByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetDecimal(_fieldIndex1)).Return(_valueOfGetDecimal);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.GetDecimal(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDecimalChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDecimal(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDecimal(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDecimal(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDecimal(null, "", 0));
        }

        [Test] public void SafeGetDecimalByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDecimal(_fieldIndex1)).Return(_valueOfGetDecimal).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldIndex1, _defaultOfGetDecimal));
            Assert.AreEqual(_defaultOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldIndex2, _defaultOfGetDecimal));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDecimalByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDecimal(_fieldIndex1)).Return(_valueOfGetDecimal).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldName1, _defaultOfGetDecimal));
            Assert.AreEqual(_defaultOfGetDecimal, DataRecordExtension.SafeGetDecimal(_dataReader, _fieldName2, _defaultOfGetDecimal));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDecimalChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDecimal(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDecimal(null, ""));
        }

        [Test] public void GetNullableDecimalByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDecimal(_fieldIndex1)).Return(_valueOfGetDecimal);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.GetNullableDecimal(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDecimal(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDecimalByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDecimal(_fieldIndex1)).Return(_valueOfGetDecimal);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDecimal, DataRecordExtension.GetNullableDecimal(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDecimal(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetDouble

        private const double _valueOfGetDouble = 8;
        private const double _defaultOfGetDouble = 100;
        private const double _defaultDouble = 0;

        [Test] public void GetDoubleChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetDouble(null, ""));
        }

        [Test] public void GetDoubleByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetDouble(_fieldIndex1)).Return(_valueOfGetDouble);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.GetDouble(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDoubleChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDouble(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDouble(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDouble(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetDouble(null, "", 0));
        }

        [Test] public void SafeGetDoubleByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDouble(_fieldIndex1)).Return(_valueOfGetDouble).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldIndex1, _defaultOfGetDouble));
            Assert.AreEqual(_defaultOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldIndex2, _defaultOfGetDouble));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetDoubleByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetDouble(_fieldIndex1)).Return(_valueOfGetDouble).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldName1, _defaultOfGetDouble));
            Assert.AreEqual(_defaultOfGetDouble, DataRecordExtension.SafeGetDouble(_dataReader, _fieldName2, _defaultOfGetDouble));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDoubleChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDouble(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableDouble(null, ""));
        }

        [Test] public void GetNullableDoubleByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDouble(_fieldIndex1)).Return(_valueOfGetDouble);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.GetNullableDouble(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDouble(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableDoubleByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetDouble(_fieldIndex1)).Return(_valueOfGetDouble);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetDouble, DataRecordExtension.GetNullableDouble(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableDouble(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetFloat

        private const float _valueOfGetFloat = 8;
        private const float _defaultOfGetFloat = 100;
        private const float _defaultFloat = 0;

        [Test] public void GetFloatChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetFloat(null, ""));
        }

        [Test] public void GetFloatByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetFloat(_fieldIndex1)).Return(_valueOfGetFloat);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.GetFloat(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetFloatChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetFloat(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetFloat(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetFloat(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetFloat(null, "", 0));
        }

        [Test] public void SafeGetFloatByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetFloat(_fieldIndex1)).Return(_valueOfGetFloat).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldIndex1, _defaultOfGetFloat));
            Assert.AreEqual(_defaultOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldIndex2, _defaultOfGetFloat));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetFloatByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetFloat(_fieldIndex1)).Return(_valueOfGetFloat).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldName1, _defaultOfGetFloat));
            Assert.AreEqual(_defaultOfGetFloat, DataRecordExtension.SafeGetFloat(_dataReader, _fieldName2, _defaultOfGetFloat));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableFloatChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableFloat(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableFloat(null, ""));
        }

        [Test] public void GetNullableFloatByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetFloat(_fieldIndex1)).Return(_valueOfGetFloat);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.GetNullableFloat(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableFloat(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableFloatByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetFloat(_fieldIndex1)).Return(_valueOfGetFloat);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetFloat, DataRecordExtension.GetNullableFloat(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableFloat(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetInt16

        private const short _valueOfGetInt16 = 8;
        private const short _defaultOfGetInt16 = 100;
        private const short _defaultInt16 = 0;

        [Test] public void GetInt16ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetInt16(null, ""));
        }

        [Test] public void GetInt16ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetInt16(_fieldIndex1)).Return(_valueOfGetInt16);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.GetInt16(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt16ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt16(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt16(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt16(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt16(null, "", 0));
        }

        [Test] public void SafeGetInt16ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt16(_fieldIndex1)).Return(_valueOfGetInt16).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldIndex1, _defaultOfGetInt16));
            Assert.AreEqual(_defaultOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldIndex2, _defaultOfGetInt16));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt16ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt16(_fieldIndex1)).Return(_valueOfGetInt16).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldName1, _defaultOfGetInt16));
            Assert.AreEqual(_defaultOfGetInt16, DataRecordExtension.SafeGetInt16(_dataReader, _fieldName2, _defaultOfGetInt16));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt16ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt16(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt16(null, ""));
        }

        [Test] public void GetNullableInt16ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt16(_fieldIndex1)).Return(_valueOfGetInt16);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.GetNullableInt16(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt16(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt16ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt16(_fieldIndex1)).Return(_valueOfGetInt16);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt16, DataRecordExtension.GetNullableInt16(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt16(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetInt32

        private const int _valueOfGetInt32 = 8;
        private const int _defaultOfGetInt32 = 100;
        private const int _defaultInt32 = 0;

        [Test] public void GetInt32ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetInt32(null, ""));
        }

        [Test] public void GetInt32ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetInt32(_fieldIndex1)).Return(_valueOfGetInt32);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.GetInt32(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt32ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt32(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt32(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt32(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt32(null, "", 0));
        }

        [Test] public void SafeGetInt32ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt32(_fieldIndex1)).Return(_valueOfGetInt32).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldIndex1, _defaultOfGetInt32));
            Assert.AreEqual(_defaultOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldIndex2, _defaultOfGetInt32));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt32ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt32(_fieldIndex1)).Return(_valueOfGetInt32).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldName1, _defaultOfGetInt32));
            Assert.AreEqual(_defaultOfGetInt32, DataRecordExtension.SafeGetInt32(_dataReader, _fieldName2, _defaultOfGetInt32));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt32ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt32(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt32(null, ""));
        }

        [Test] public void GetNullableInt32ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt32(_fieldIndex1)).Return(_valueOfGetInt32);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.GetNullableInt32(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt32(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt32ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt32(_fieldIndex1)).Return(_valueOfGetInt32);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt32, DataRecordExtension.GetNullableInt32(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt32(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetInt64

        private const long _valueOfGetInt64 = 8;
        private const long _defaultOfGetInt64 = 100;
        private const long _defaultInt64 = 0;

        [Test] public void GetInt64ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetInt64(null, ""));
        }

        [Test] public void GetInt64ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetInt64(_fieldIndex1)).Return(_valueOfGetInt64);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.GetInt64(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt64ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt64(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt64(null, 0, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt64(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetInt64(null, "", 0));
        }

        [Test] public void SafeGetInt64ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt64(_fieldIndex1)).Return(_valueOfGetInt64).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldIndex1, _defaultOfGetInt64));
            Assert.AreEqual(_defaultOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldIndex2, _defaultOfGetInt64));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetInt64ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetInt64(_fieldIndex1)).Return(_valueOfGetInt64).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldName1, _defaultOfGetInt64));
            Assert.AreEqual(_defaultOfGetInt64, DataRecordExtension.SafeGetInt64(_dataReader, _fieldName2, _defaultOfGetInt64));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt64ChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt64(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetNullableInt64(null, ""));
        }

        [Test] public void GetNullableInt64ByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt64(_fieldIndex1)).Return(_valueOfGetInt64);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.GetNullableInt64(_dataReader, _fieldIndex1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt64(_dataReader, _fieldIndex2));
            _mockery.VerifyAll();
        }

        [Test] public void GetNullableInt64ByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false);
            Expect.Call(_dataReader.GetInt64(_fieldIndex1)).Return(_valueOfGetInt64);
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2);
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetInt64, DataRecordExtension.GetNullableInt64(_dataReader, _fieldName1));
            Assert.AreEqual(null, DataRecordExtension.GetNullableInt64(_dataReader, _fieldName2));
            _mockery.VerifyAll();
        }

        #endregion

        #region GetString

        private const string _valueOfGetString = "_valueOfGetString";
        private const string _defaultOfGetString = "_defaultOfGetString";
        private const string _defaultString = null;

        [Test] public void GetStringChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.GetString(null, ""));
        }

        [Test] public void GetStringByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1);
            Expect.Call(_dataReader.GetString(_fieldIndex1)).Return(_valueOfGetString);
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetString, DataRecordExtension.GetString(_dataReader, _fieldName1));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetStringChokesOnNullRecord()
        {
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetString(null, 0));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetString(null, 0, _defaultOfGetString));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetString(null, ""));
            Assert.Throws<NullReferenceException>(() => DataRecordExtension.SafeGetString(null, "", _defaultOfGetString));
        }

        [Test] public void SafeGetStringByIndex()
        {
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetString(_fieldIndex1)).Return(_valueOfGetString).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldIndex1));
            Assert.AreEqual(_defaultString, DataRecordExtension.SafeGetString(_dataReader, _fieldIndex2));
            Assert.AreEqual(_valueOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldIndex1, _defaultOfGetString));
            Assert.AreEqual(_defaultOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldIndex2, _defaultOfGetString));
            _mockery.VerifyAll();
        }

        [Test] public void SafeGetStringByName()
        {
            Expect.Call(_dataReader.GetOrdinal(_fieldName1)).Return(_fieldIndex1).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex1)).Return(false).Repeat.Twice();
            Expect.Call(_dataReader.GetString(_fieldIndex1)).Return(_valueOfGetString).Repeat.Twice();
            Expect.Call(_dataReader.GetOrdinal(_fieldName2)).Return(_fieldIndex2).Repeat.Twice();
            Expect.Call(_dataReader.IsDBNull(_fieldIndex2)).Return(true).Repeat.Twice();
            _mockery.ReplayAll();
            Assert.AreEqual(_valueOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldName1));
            Assert.AreEqual(_defaultString, DataRecordExtension.SafeGetString(_dataReader, _fieldName2));
            Assert.AreEqual(_valueOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldName1, _defaultOfGetString));
            Assert.AreEqual(_defaultOfGetString, DataRecordExtension.SafeGetString(_dataReader, _fieldName2, _defaultOfGetString));
            _mockery.VerifyAll();
        }

        #endregion

        private static void ExpectArguementNullExceptionForTrueValue(TestDelegate code)
        {
            ExpectArguementNullException("trueValue", code);
        }

        private static void ExpectArguementNullException(string argumentName, TestDelegate code)
        {
            var e = Assert.Throws(Is.InstanceOf<ArgumentNullException>(), code);
            StringAssert.Contains(argumentName, e.Message);
        }
    }
}