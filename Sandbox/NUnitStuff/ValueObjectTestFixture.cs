﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NUnitStuff
{

    [TestFixture]
    public class FooTest : ValueObjectTestFixture<Foo>
    {
        protected override Foo NewValueObject()
        {
            return new Foo();
        }
    }

    public class Foo : ICloneable
    {
        private int _id = -1;

        [DefaultValue(-1)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool IsGood { get; set; }

        public long LongProperty { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Foo)) return false;
            return Equals((Foo)obj);
        }

        public bool Equals(Foo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._id == _id && other.LongProperty == LongProperty;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_id * 397) ^ LongProperty.GetHashCode();
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Foo Clone()
        {
            return (Foo) MemberwiseClone();
        }
    }

    /// <summary>
    /// Base test cases for value objects.
    /// </summary>
    /// <typeparam name="T">Type of the value object to be tested.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class ValueObjectTestFixture<T>
    {
        private static readonly IEnumerable<PropertyInfo> _allProperties;

        private readonly List<string> _exclusions = new List<string>();

        private static readonly Func<T, T, bool> _typedEquals;

        private static readonly bool _isCloneable;

        static ValueObjectTestFixture()
        {
            _allProperties = typeof(T).GetProperties();
            var typedEquals = typeof(T).GetMethod("Equals", new[] {typeof (T)});
            if (typedEquals != null && typedEquals.GetParameters()[0].ParameterType == typeof(T))
            {
                _typedEquals = (Func<T, T, bool>) Delegate.CreateDelegate(typeof(Func<T, T, bool>), typedEquals);
            }

            _isCloneable = (typeof (ICloneable).IsAssignableFrom(typeof (T)));
        }

        /// <summary>
        /// Sets an instance of <see cref="IMockProvider"/> that is used by 
        /// <see cref="TestData"/> to generate data points for non build-in
        /// data types.
        /// </summary>
        public IMockProvider MockProvider { private get; set; }

        /// <summary>
        /// Properties that will be used in the test of
        /// <see cref="PropertyInitializeAccordingToTheDefaultValueAttributeOrTypeDefault"/>.
        /// By default, this returns all readable public properties. Derived 
        /// class can override this method to filter or provide a different set.
        /// </summary>
        /// <returns>Properties to be check for default value.</returns>
        public virtual IEnumerable<PropertyInfo> DefaultTestCandidates()
        {
            return from pi in _allProperties
                   where pi.CanRead && !_exclusions.Contains(pi.Name)
                   select pi;
        }

        /// <summary>
        /// Properties that will be used in the test of
        /// <see cref="CanSetPropertyValueAndReadBackSameValue"/>.
        /// By default, this returns all read/write public properties. Derived 
        /// class can override this method to filter or provide a different set.
        /// </summary>
        /// <returns>Properties to be tested with read write access.</returns>
        public virtual IEnumerable<PropertyInfo> ReadWriteTestCandidates()
        {
            return from pi in _allProperties
                   where pi.CanRead && pi.CanWrite && !_exclusions.Contains(pi.Name)
                   select pi;
        }

        /// <summary>
        /// Properties that will be used in the test of
        /// <see cref="NotEqualsWhenPropertyIsDifferent"/>.
        /// By default, this returns all writable public properties. Derived 
        /// class can override this method to filter or provide a different set.
        /// </summary>
        /// <returns>Properties to be check for equals.</returns>
        public virtual IEnumerable<PropertyInfo> EqualsTestCandidates()
        {
            return from pi in _allProperties
                   where pi.CanWrite && !_exclusions.Contains(pi.Name)
                   select pi;
        }

        /// <summary>
        /// Properties that will be used in the test of <see cref="ClonesAllProperties"/>.
        /// By default, this returns <see cref="ReadWriteTestCandidates"/>. 
        /// Derived class can override this method to filter or provide a different set.
        /// </summary>
        /// <returns>Properties to be check for clone result.</returns>
        public virtual IEnumerable<PropertyInfo> ClonedProperties()
        {
            return ReadWriteTestCandidates();
        }

        /// <summary>
        /// This is used to control test framework. Do not user directly
        /// </summary>
        /// <returns></returns>
        [Obsolete("For test framework only")]
        public IEnumerable<string> HasTypedEquals()
        {
            return _typedEquals == null ? new string[0] : new[] {"Typed"};
        }

        /// <summary>
        /// This is used to control test framework. Do not user directly
        /// </summary>
        /// <returns></returns>
        [Obsolete("For test framework only")]
        public IEnumerable<string> IsCloneable()
        {
            return _isCloneable ? new[] { "Cloneable" } : new string[0];
        }

        /// <summary>
        /// To verify the default value of each property. The Properties to be
        /// tested is defined by <see cref="DefaultTestCandidates"/>. The
        /// default value is expected to be either the value specified by the
        /// <see cref="DefaultValueAttribute"/> if one exists, or the default
        /// value of the property type.
        /// </summary>
        /// <param name="property">The property being tested.</param>
        [Test]
        public virtual void PropertyInitializeAccordingToTheDefaultValueAttributeOrTypeDefault(
            [ValueSource("DefaultTestCandidates")] PropertyInfo property)
        {
            const string message = 
                "Default value of property {0} must equals to the value specified "+
                "by DefaultValueAttribute, or default value of the type.";
            var sut = NewValueObject();

            var defaultValue = GetDefaultValueAttribute(property);
            var value = defaultValue != null
                               ? defaultValue.Value
                               : (property.PropertyType.IsValueType
                                      ? Activator.CreateInstance(property.PropertyType)
                                      : null);
            Assert.That(property.GetValue(sut, null), Is.EqualTo(value), message, property.Name);
        }

        /// <summary>
        /// To verify seach property supplied by <see cref="ReadWriteTestCandidates"/>
        /// can be set by the values provided by <see cref="TestData"/> and
        /// the same value can be later read back.
        /// </summary>
        /// <param name="property">The property being tested.</param>
        [Test]
        public virtual void CanSetPropertyValueAndReadBackSameValue(
            [ValueSource("ReadWriteTestCandidates")] PropertyInfo property)
        {
                var sut = NewValueObject();
                var testValues = TestData(property);

                foreach (object value in testValues)
                {
                    try
                    {
                        property.SetValue(sut, value, null);
                    }
                    catch (Exception e)
                    {
                        throw new AssertionException(
                            "Unable to set property " + property.Name +
                            " to value " + value + ": " + e.Message, e);
                    }
                    Assert.That(property.GetValue(sut, null), Is.EqualTo(value),
                        property.Name + ": value read back doesn't equals to what was set.");
                }
        }

        /// <summary>
        /// To verify that the value object doesn't equals to null.
        /// </summary>
        [Test] public virtual void NotEquasNull()
        {
            var sut = NewValueObject();
            Assert.IsFalse(sut.Equals(null));
        }

        /// <summary>
        /// To verify that the typed equals method of the value object returns
        /// false when compared to null. Only applyed when the value object is
        /// reference type.
        /// </summary>
        /// <param name="dummy">For test framework to skip value type.</param>
        [Test] public virtual void NotEqualsNull([ValueSource("HasTypedEquals")] string dummy)
        {
            if(typeof(T).IsValueType) return;
            var sut = NewValueObject();
            Assert.IsFalse(_typedEquals(sut, default(T)));
        }

        /// <summary>
        /// To verify that the value object doesn't equals to an arbitrary object.
        /// </summary>
        [Test] public virtual void NotEquasNewObject()
        {
            var sut = NewValueObject();
            Assert.IsFalse(sut.Equals(new object()));
        }

        /// <summary>
        /// To verify that the value object does equals to itself.
        /// </summary>
        [Test] public virtual void EqualsToSelf()
        {
            var sut = NewValueObject();
            Assert.IsTrue(sut.Equals(sut));
        }

        /// <summary>
        /// To verify that the typed equals method return true for the value
        /// object itself.
        /// </summary>
        /// <param name="dummy">For test framework to skip value type.</param>
        [Test] public virtual void EqualsToSelf([ValueSource("HasTypedEquals")] string dummy)
        {
            var sut = NewValueObject();
            Assert.IsTrue(_typedEquals(sut, sut));
        }

        /// <summary>
        /// To verify that all properties provided by <see cref="ClonedProperties"/>
        /// are copied as the result of <see cref="ICloneable.Clone"/> if the
        /// value object implements <see cref="ICloneable"/>.
        /// </summary>
        /// <param name="dummy">For test framework to skip non-cloneable.</param>
        [Test]
        public virtual void ClonesAllProperties([ValueSource("IsCloneable")] string dummy)
        {
            var sut = NewValueObject();
            var sut2 = NewValueObject();
            foreach (var property in ClonedProperties())
            {
                var testValues = TestData(property).GetEnumerator();
                property.SetValue(sut, GetDataPoint(property, testValues), null);
                property.SetValue(sut2, GetDataPoint(property, testValues), null);
            }

            var clone = ((ICloneable)sut).Clone();
            var clone2 = ((ICloneable)sut2).Clone();
            foreach (var property in ClonedProperties())
            {
                Assert.That(
                    property.GetValue(clone, null), 
                    Is.EqualTo(property.GetValue(sut, null)), 
                    "Value of property {0} is different in cloned object.",
                    property.Name);
                Assert.That(
                    property.GetValue(clone2, null),
                    Is.EqualTo(property.GetValue(sut2, null)),
                    "Value of property {0} is different in cloned object.",
                    property.Name);
            }
        }

        /// <summary>
        /// To verify that two value objects do not equal if any property
        /// supplied by <see cref="EqualsTestCandidates"/> are different.
        /// </summary>
        /// <param name="property">Property being tested.</param>
        [Test]
        public virtual void NotEqualsWhenPropertyIsDifferent(
            [ValueSource("EqualsTestCandidates")] PropertyInfo property)
        {
            var sut = NewValueObject();
            var testValues = TestData(property).GetEnumerator();

            var sutValue = GetDataPoint(property, testValues);
            property.SetValue(sut, sutValue, null);

            // 2nd data point
            AssertNotEquals(property, testValues, sut, sutValue);
            // 3rd data point
            AssertNotEquals(property, testValues, sut, sutValue);
        }

        private void AssertNotEquals(PropertyInfo property, IEnumerator testValues, T sut, object sutValue)
        {
            var sut2 = NewValueObject();
            var sut2Value = GetDataPoint(property, testValues);
            property.SetValue(sut2, sut2Value, null);

            const string message = "Value object equals when property {0} are not. {1} vs. {2}.";
            Assert.IsFalse(sut.Equals(sut2), message, property.Name, sutValue, sut2Value);
        }

        /// <summary>
        /// To verify that, if two value objects has same property value and 
        /// <see cref="object.Equals(object)"/> returns true, then
        /// <see cref="object.GetHashCode"/> must be same. Participating
        /// properties are from <see cref="EqualsTestCandidates"/>.
        /// </summary>
        /// <param name="property">The property being tested</param>
        [Test]
        public void GetHashCodeReturnsSameHashWhenEquals(
            [ValueSource("EqualsTestCandidates")] PropertyInfo property)
        {
            var sut = NewValueObject();
            var another = NewValueObject();

            var testValues = TestData(property).GetEnumerator();

            AssertGetHashCode(property, sut, another, GetDataPoint(property, testValues));
            // 2nd data point
            AssertGetHashCode(property, sut, another, GetDataPoint(property, testValues));
        }

        private static void AssertGetHashCode(PropertyInfo property, T sut, T another, object value)
        {
            property.SetValue(sut, value, null);
            property.SetValue(another, value, null);
            if (sut.Equals(another))
            {
                Assert.That(sut.GetHashCode(), Is.EqualTo(another.GetHashCode()),
                            "GetHashCode() must return same value when object equals.");
            }
        }

        /// <summary>
        /// Derived class must implemented to provide a new instance of value
        /// object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        protected abstract T NewValueObject();

        /// <summary>
        /// To specify properties to be excluded from all tests.
        /// </summary>
        /// <param name="properties">Properties to exclude.</param>
        protected void ExcludeProperties(IEnumerable<string> properties)
        {
            _exclusions.AddRange(properties);
        }

        /// <summary>
        /// To specify properties to be excluded from all tests.
        /// </summary>
        /// <param name="properties">Properties to exclude.</param>
        protected void ExcludeProperties(params string[] properties)
        {
            _exclusions.AddRange(properties);
        }

        /// <summary>
        /// Provided test data points for a given <paramref name="property"/>.
        /// Minimal three data points are required. The default implementation
        /// returns data point depends on type of the property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation provide data points based on the type of the 
        /// property. All <see href="http://msdn.microsoft.com/en-us/library/cs7y5x0x.aspx">
        /// build-in data types</see> and there corresponding <see cref="Nullable{T}"/>s 
        /// are supported as well as <see cref="DateTime"/> and <see cref="TimeSpan"/>.
        /// </para>
        /// <para>
        /// If derived class has property returns types that are not supported, 
        /// it must set the <see cref="MockProvider"/> property and/or override 
        /// this method to provide data points for those properties.
        /// </para>
        /// <para>
        /// <b>Important Note:</b>
        /// When providing data points, make sure that, a) minimal three data
        /// points are required; b) the second data point must be the default
        /// value for value type and null for reference type.
        /// </para>
        /// </remarks>
        /// <param name="property">
        /// Property for which the test data points will be used for.
        /// </param>
        /// <returns>
        /// Minimal three test data should be returned, the 2nd being null for
        /// reference type and default value for value type.
        /// </returns>
        protected virtual IEnumerable TestData(PropertyInfo property)
        {
            var type = property.PropertyType;

            #region bool related
            if (type == typeof(bool))
            {
                return new bool[] { true, false, false };
            }
            if (type == typeof(bool?))
            {
                return new bool?[] { true, null, false };
            }
            #endregion

            #region byte related
            if (type == typeof(byte))
            {
                return new byte[] { 0xFE, 0, 0x01, byte.MaxValue, byte.MinValue };
            }
            if (type == typeof(byte?))
            {
                return new byte?[] { 0xFE, null, 0x01, byte.MaxValue, byte.MinValue };
            }
            if (type == typeof(sbyte))
            {
                return new sbyte[] { -1, 0, 0x01, sbyte.MaxValue, sbyte.MinValue };
            }
            if (type == typeof(sbyte?))
            {
                return new sbyte?[] { -1, null, 0x01, sbyte.MaxValue, sbyte.MinValue };
            }
            #endregion

            #region short related
            if (type == typeof(short))
            {
                return new short[] { -1, 0, 1, short.MaxValue, short.MinValue };
            }
            if (type == typeof(short?))
            {
                return new short?[] { -1, null, 1, short.MaxValue, short.MinValue };
            }
            if (type == typeof(ushort))
            {
                return new ushort[] { 0xfffe, 0, 1, ushort.MaxValue, ushort.MinValue };
            }
            if (type == typeof(ushort?))
            {
                return new ushort?[] { 0xfffe, null, 1, ushort.MaxValue, ushort.MinValue };
            } 
            #endregion

            #region char related
            if (type == typeof(char))
            {
                return new char[] { '0', '\0', 'a', '华', char.MaxValue, char.MinValue };
            }
            if (type == typeof(char?))
            {
                return new char?[] { '0', null, 'a', '华', char.MaxValue, char.MinValue };
            } 
            #endregion

            #region int related
            if (type == typeof(int))
            {
                return new int[] { -1, 0, 1, int.MaxValue, int.MinValue };
            }
            if (type == typeof(int?))
            {
                return new int?[] { -1, null, 1, int.MaxValue, int.MinValue };
            }
            if (type == typeof(uint))
            {
                return new uint[] { 0xffee, 0, 1, uint.MaxValue, uint.MinValue };
            }
            if (type == typeof(uint?))
            {
                return new uint?[] { 0xffee, null, 1, uint.MaxValue, uint.MinValue };
            } 
            #endregion

            #region long related
            if (type == typeof(long))
            {
                return new long[] { -1, 0, 1, long.MaxValue, long.MinValue };
            }
            if (type == typeof(long?))
            {
                return new long?[] { -1, null, 1, long.MaxValue, long.MinValue };
            }
            if (type == typeof(ulong))
            {
                return new ulong[] { 0xffee, 0, 1, ulong.MaxValue, ulong.MinValue };
            }
            if (type == typeof(long?))
            {
                return new ulong?[] { 0xffee, null, 1, ulong.MaxValue, ulong.MinValue };
            }
            #endregion

            #region float related
            if (type == typeof(float))
            {
                return new float[]
                           {
                               -0.1f, 0, 0.1f, float.NaN, 
                               float.PositiveInfinity, float.NegativeInfinity, 
                               float.MaxValue, float.MinValue
                           };
            }
            if (type == typeof(float?))
            {
                return new float?[]
                           {
                               -0.1f, null, 0.1f, float.NaN, 
                               float.PositiveInfinity, float.NegativeInfinity, 
                               float.MaxValue, float.MinValue
                           };
            } 
            #endregion

            #region double related
            if (type == typeof(double))
            {
                return new double[]
                           {
                               -0.1, 0, 0.1, double.NaN, 
                               double.PositiveInfinity, double.NegativeInfinity,
                               double.MaxValue, double.MinValue
                           };
            }
            if (type == typeof(double?))
            {
                return new double?[]
                           {
                               -0.1, null, 0.1, double.NaN, 
                               double.PositiveInfinity, double.NegativeInfinity,
                               double.MaxValue, double.MinValue
                           };
            }
            #endregion

            #region decimal related
            if (type == typeof(decimal))
            {
                return new decimal[]
                           {
                               decimal.Negate(decimal.One), decimal.Zero, decimal.One, 
                               decimal.MaxValue, decimal.MinValue
                           };
            }
            if (type == typeof(decimal?))
            {
                return new decimal?[]
                           {
                               decimal.Negate(decimal.One), null, decimal.One, 
                               decimal.MaxValue, decimal.MinValue
                           };
            }
            #endregion

            #region TimeSpan related
            if (type == typeof(TimeSpan))
            {
                return new TimeSpan[]
                           {
                               TimeSpan.FromSeconds(-1), 
                               TimeSpan.Zero,
                               TimeSpan.FromSeconds(1), 
                               TimeSpan.MaxValue, 
                               TimeSpan.MinValue
                           };
            }
            if (type == typeof(TimeSpan?))
            {
                return new TimeSpan?[]
                           {
                               TimeSpan.FromSeconds(-1), 
                               null,
                               TimeSpan.FromSeconds(1), 
                               TimeSpan.MaxValue, 
                               TimeSpan.MinValue
                           };
            } 
            #endregion

            #region DateTime related
            if (type == typeof(DateTime))
            {
                var oneHour = TimeSpan.FromHours(1);
                var now = DateTime.UtcNow;
                return new DateTime[]
                           {
                               now - oneHour,
                               new DateTime(),
                               now + oneHour,
                               now,
                               DateTime.MaxValue,
                               DateTime.MinValue
                           };
            }
            if (type == typeof(DateTime?))
            {
                var oneHour = TimeSpan.FromHours(1);
                var now = DateTime.UtcNow;
                return new DateTime?[]
                           {
                               now - oneHour,
                               null,
                               now + oneHour,
                               now,
                               DateTime.MaxValue,
                               DateTime.MinValue
                           };
            } 
            #endregion

            if (type == typeof(string))
            {
                return new[] { "894329034", null, "tiny", @"""", @"this 
is 
a 
multi 
line
string"};
            }

            if (type == typeof(object))
            {
                return new object[] { new object(), null, new object()};
            }

            return new object[0];
        }

        private static DefaultValueAttribute GetDefaultValueAttribute(ICustomAttributeProvider property)
        {
            var defaultValues = property.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            return (defaultValues != null && defaultValues.Length > 0)
                       ? ((DefaultValueAttribute) defaultValues[0])
                       : null;
        }

        private static object GetDataPoint(PropertyInfo property, IEnumerator testValues)
        {
            Assert.IsTrue(testValues.MoveNext(),
                          "Missing test data. Please suppy minimal three test data point " +
                          "for property {0} by overriding TestValues method. The 2nd data " +
                          "point should be null or default value of value type.",
                          property.Name);
            return testValues.Current;
        }
    }
}
