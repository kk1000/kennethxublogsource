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
using NUnit.Framework;

namespace NUnitStuff
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
