using System;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test case of using <see cref="ValueObjectTestFixture{T}"/> on value
    /// type objects. 
    /// </summary>
    [TestFixture]
    public class ValueTypeTest : NewableValueObjectTestFixture<ValueTypeTest.StructObject>
    {
        public struct StructObject : ICloneable
        {
            public int Id { get; set; }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(StructObject other)
            {
                return other.Id == Id;
            }

            public override int GetHashCode()
            {
                return Id;
            }

            object ICloneable.Clone()
            {
                return Clone();
            }

            public StructObject Clone()
            {
                return (StructObject) MemberwiseClone();
            }
        }
    }
}
