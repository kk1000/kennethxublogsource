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
using System.Collections.Generic;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test the <see cref="RhinoMocksTestDataProvider"/>.
    /// </summary>
    /// <autorh>Kenneth Xu</autorh>
    [TestFixture]
    public class RhinoMocksProviderTest : NewableValueObjectTestFixture<RhinoMocksProviderTest.ValueObject>
    {
        public RhinoMocksProviderTest()
        {
            TestDataProvider = new RhinoMocksTestDataProvider();
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
