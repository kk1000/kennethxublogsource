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
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test case of using <see cref="ValueObjectTestFixture{T}"/> on value
    /// type objects. 
    /// </summary>
    /// <author>Kenneth Xu</author>
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
