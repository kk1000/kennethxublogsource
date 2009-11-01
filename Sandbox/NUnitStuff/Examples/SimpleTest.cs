using System;
using System.ComponentModel;
using NUnit.Framework;

namespace NUnitStuff.Examples
{
    /// <summary>
    /// A Simple use case of <see cref="NewableValueObjectTestFixture{T}"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture]
    public class SimpleTest : NewableValueObjectTestFixture<SimpleTest.Foo>
    {

        public class Foo : ICloneable
        {
            private int _id = -1;

            [DefaultValue(-1)]
            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string StringProperty { get; set; }

            public long LongProperty { get; set; }

            object ICloneable.Clone()
            {
                return Clone();
            }

            public Foo Clone()
            {
                return (Foo)MemberwiseClone();
            }

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
                return other._id == _id &&
                    Equals(other.StringProperty, StringProperty) &&
                    other.LongProperty == LongProperty;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = _id;
                    result = (result * 397) ^ (StringProperty != null ? StringProperty.GetHashCode() : 0);
                    result = (result * 397) ^ LongProperty.GetHashCode();
                    return result;
                }
            }
        }
    }
}
