using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test the <see cref="RhinoMockTestDataProvider"/>.
    /// </summary>
    /// <autorh>Kenneth Xu</autorh>
    [TestFixture]
    public class MockProviderTest : NewableValueObjectTestFixture<MockProviderTest.ValueObject>
    {
        public MockProviderTest()
        {
            MockProvider = new RhinoMockTestDataProvider();
        }

        public class ValueObject
        {
            public IList<string> StringIList { get; set; }

            public List<TimeSpan> TimeSpanList { get; set; }

            public ICollection<int> IntICollection { get; set; }

            public IDictionary<string, object> StringObjectIDictrionary { get; set; }

            public Dictionary<int, DateTime> IntDataTimeDictionary { get; set; }

            public Component Component { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(ValueObject)) return false;
                return Equals((ValueObject)obj);
            }

            public bool Equals(ValueObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.StringIList, StringIList) &&
                    Equals(other.TimeSpanList, TimeSpanList) &&
                    Equals(other.IntICollection, IntICollection) &&
                    Equals(other.StringObjectIDictrionary, StringObjectIDictrionary) &&
                    Equals(other.IntDataTimeDictionary, IntDataTimeDictionary) &&
                    Equals(other.Component, Component);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (StringIList != null ? StringIList.GetHashCode() : 0);
                    result = (result * 397) ^ (TimeSpanList != null ? TimeSpanList.GetHashCode() : 0);
                    result = (result * 397) ^ (IntICollection != null ? IntICollection.GetHashCode() : 0);
                    result = (result * 397) ^ (StringObjectIDictrionary != null ? StringObjectIDictrionary.GetHashCode() : 0);
                    result = (result * 397) ^ (IntDataTimeDictionary != null ? IntDataTimeDictionary.GetHashCode() : 0);
                    result = (result * 397) ^ (Component != null ? Component.GetHashCode() : 0);
                    return result;
                }
            }
        }

        public class Component
        {
            public string Id { get; set; }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(Component other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Id, Id);
            }

            public override int GetHashCode()
            {
                return (Id != null ? Id.GetHashCode() : 0);
            }
        }
    }
}
