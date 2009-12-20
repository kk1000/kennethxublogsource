#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
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
using System.ComponentModel;

namespace NUnitStuff
{
    /// <summary>
    /// Test the functionality of <see cref="NewableValueObjectTestFixture{T}"/>
    /// and <see cref="ValueObjectTestFixture{T}"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class AllBuildInTest : NewableValueObjectTestFixture<AllBuildInTest.AllBuildIn>
    {
        public class AllBuildIn : ICloneable
        {
            public bool Bool { get; set; }

            public bool? NullableBool { get; set; }

            private int _int1 = -1;
            [DefaultValue(-1)]
            public int Int1
            {
                get { return _int1; }
                set { _int1 = value; }
            }

            public int Int { get; set; }

            public uint Uint { get; set; }

            public int? NullableInt { get; set; }

            public uint? NullableUint { get; set; }

            public byte Byte { get; set; }

            public sbyte Sbyte { get; set; }

            public byte? NullableByte { get; set; }

            public sbyte? NullableSbyte { get; set; }

            public short Short { get; set; }

            public ushort Ushort { get; set; }

            public short? NullableShort { get; set; }

            public ushort? NullableUshort { get; set; }

            public long Long { get; set; }

            public ulong Ulong { get; set; }

            public long? NullableLong { get; set; }

            public ulong? NullableUlong { get; set; }

            public char Char { get; set; }

            public char? NullableChar { get; set; }

            public float Float { get; set; }

            public float? NullableFloat { get; set; }

            public double Double { get; set; }

            public double? NullableDouble { get; set; }

            public decimal Decimal { get; set; }

            public decimal? NullableDecimal { get; set; }

            public string String { get; set; }

            public object Object { get; set; }

            public TimeSpan TimeSpan { get; set; }

            public TimeSpan? NullableTimeSpan { get; set; }

            public DateTime DateTime { get; set; }

            public DateTime? NullableDateTime { get; set; }

            object ICloneable.Clone()
            {
                return Clone();
            }

            public AllBuildIn Clone()
            {
                return (AllBuildIn)MemberwiseClone();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(AllBuildIn)) return false;
                return Equals((AllBuildIn)obj);
            }

            public bool Equals(AllBuildIn other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other._int1 == _int1 &&
                    other.Bool.Equals(Bool) &&
                    other.NullableBool.Equals(NullableBool) &&
                    other.Int == Int &&
                    other.Uint == Uint &&
                    other.NullableInt.Equals(NullableInt) &&
                    other.NullableUint.Equals(NullableUint) &&
                    other.Byte == Byte &&
                    other.Sbyte == Sbyte &&
                    other.NullableByte.Equals(NullableByte) &&
                    other.NullableSbyte.Equals(NullableSbyte) &&
                    other.Short == Short &&
                    other.Ushort == Ushort &&
                    other.NullableShort.Equals(NullableShort) &&
                    other.NullableUshort.Equals(NullableUshort) &&
                    other.Long == Long &&
                    other.Ulong == Ulong &&
                    other.NullableLong.Equals(NullableLong) &&
                    other.NullableUlong.Equals(NullableUlong) &&
                    other.Char == Char &&
                    other.NullableChar.Equals(NullableChar) &&
                    other.Float == Float &&
                    other.NullableFloat.Equals(NullableFloat) &&
                    other.Double == Double &&
                    other.NullableDouble.Equals(NullableDouble) &&
                    other.Decimal == Decimal &&
                    other.NullableDecimal.Equals(NullableDecimal) &&
                    Equals(other.String, String) &&
                    Equals(other.Object, Object) &&
                    other.TimeSpan.Equals(TimeSpan) &&
                    other.NullableTimeSpan.Equals(NullableTimeSpan) &&
                    other.DateTime.Equals(DateTime) &&
                    other.NullableDateTime.Equals(NullableDateTime);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = _int1;
                    result = (result * 397) ^ Bool.GetHashCode();
                    result = (result * 397) ^ (NullableBool.HasValue ? NullableBool.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Int;
                    result = (result * 397) ^ Uint.GetHashCode();
                    result = (result * 397) ^ (NullableInt.HasValue ? NullableInt.Value : 0);
                    result = (result * 397) ^ (NullableUint.HasValue ? NullableUint.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Byte.GetHashCode();
                    result = (result * 397) ^ Sbyte.GetHashCode();
                    result = (result * 397) ^ (NullableByte.HasValue ? NullableByte.Value.GetHashCode() : 0);
                    result = (result * 397) ^ (NullableSbyte.HasValue ? NullableSbyte.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Short.GetHashCode();
                    result = (result * 397) ^ Ushort.GetHashCode();
                    result = (result * 397) ^ (NullableShort.HasValue ? NullableShort.Value.GetHashCode() : 0);
                    result = (result * 397) ^ (NullableUshort.HasValue ? NullableUshort.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Long.GetHashCode();
                    result = (result * 397) ^ Ulong.GetHashCode();
                    result = (result * 397) ^ (NullableLong.HasValue ? NullableLong.Value.GetHashCode() : 0);
                    result = (result * 397) ^ (NullableUlong.HasValue ? NullableUlong.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Char.GetHashCode();
                    result = (result * 397) ^ (NullableChar.HasValue ? NullableChar.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Float.GetHashCode();
                    result = (result * 397) ^ (NullableFloat.HasValue ? NullableFloat.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Double.GetHashCode();
                    result = (result * 397) ^ (NullableDouble.HasValue ? NullableDouble.Value.GetHashCode() : 0);
                    result = (result * 397) ^ Decimal.GetHashCode();
                    result = (result * 397) ^ (NullableDecimal.HasValue ? NullableDecimal.Value.GetHashCode() : 0);
                    result = (result * 397) ^ (String != null ? String.GetHashCode() : 0);
                    result = (result * 397) ^ (Object != null ? Object.GetHashCode() : 0);
                    result = (result * 397) ^ TimeSpan.GetHashCode();
                    result = (result * 397) ^ (NullableTimeSpan.HasValue ? NullableTimeSpan.Value.GetHashCode() : 0);
                    result = (result * 397) ^ DateTime.GetHashCode();
                    result = (result * 397) ^ (NullableDateTime.HasValue ? NullableDateTime.Value.GetHashCode() : 0);
                    return result;
                }
            }
        }
    }
}
