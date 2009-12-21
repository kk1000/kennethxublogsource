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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Serve both an exmaple of how to use <see cref="ValueObjectTestFixture{T}"/>
    /// to test customized value object, and the test case of the test fixture's 
    /// support for the customized value object.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture]
    public class CustomTest : ValueObjectTestFixture<CustomTest.CustomObject>
    {
        public CustomTest()
        {
            // Exclude the Name property from being tested by ValueObjectTestFixture.
            ExcludeProperties("Name");
        }

        public override IEnumerable<PropertyInfo> EqualsTestCandidates()
        {
            // Exclude the Tag property from equaity test.
            return from p in base.EqualsTestCandidates() where p.Name != "Tag" select p;
        }

        [Test] public void NamePropertyReturnsTheNameSetInConstructor()
        {
            const string name = "832jfs";
            Assert.That(new CustomObject(name).Name, Is.EqualTo(name));
        }

        [TestCase(3, 4, Operation.Addition, Result = 7)]
        [TestCase(3, 4, Operation.Subtraction, Result = -1)]
        [TestCase(3, 4, Operation.Muiplication, Result = 12)]
        [TestCase(12, 4, Operation.Division, Result = 3)]
        public int ResultIsCalculatedFromOpernedsAndOperator(int left, int right, Operation operation)
        {
            var c = NewValueObject();
            c.LeftOperand = left;
            c.RightOperand = right;
            c.Operation = operation;
            return c.Result;
        }

        protected override CustomObject NewValueObject()
        {
            return new CustomObject("TestObject");
        }

        protected override IEnumerable TestData(PropertyInfo property)
        {
            // ValueObjectTestFixture doesn't support enum at this moment
            // so we had to provide our own test data.
            if (property.PropertyType == typeof(Operation))
            {
                return new[]
                           {
                               Operation.Subtraction, 
                               new Operation(), 
                               Operation.Addition, 
                               Operation.Muiplication, 
                               Operation.Division
                           };
            }
            return base.TestData(property);
        }

        public class CustomObject
        {
            private readonly string _name;
            public CustomObject(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } }

            public int LeftOperand { get; set; }

            public int RightOperand { get; set; }

            public Operation Operation { get; set; }

            public object Tag { get; set; }

            public int Result
            {
                get
                {
                    switch (Operation)
                    {
                        case Operation.Addition:
                            return LeftOperand + RightOperand;
                        case Operation.Subtraction:
                            return LeftOperand - RightOperand;
                        case Operation.Muiplication:
                            return LeftOperand * RightOperand;
                        case Operation.Division:
                            return LeftOperand / RightOperand;
                        default:
                            throw new InvalidOperationException("Unsupported operation " + Operation);
                    }
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(CustomObject)) return false;
                return Equals((CustomObject)obj);
            }

            public bool Equals(CustomObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other._name, _name) &&
                    other.LeftOperand == LeftOperand &&
                    other.RightOperand == RightOperand &&
                    Equals(other.Operation, Operation);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (_name != null ? _name.GetHashCode() : 0);
                    result = (result * 397) ^ LeftOperand;
                    result = (result * 397) ^ RightOperand;
                    result = (result * 397) ^ Operation.GetHashCode();
                    return result;
                }
            }
        }

        public enum Operation
        {
            Addition,
            Subtraction,
            Muiplication,
            Division
        }
    }
}
