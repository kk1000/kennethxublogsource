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
using System.Reflection;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test cases and exmaple of using <see cref="ValueObjectTestFixture{T}"/>
    /// to test implementation of <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class BadNotifyPropertyChangedTest 
        : NewableValueObjectTestFixture<BadNotifyPropertyChangedTest.BadFoo>
    {

        public override System.Collections.Generic.IEnumerable<PropertyInfo> EqualsTestCandidates()
        {
            return new PropertyInfo[0];
        }

        internal override void DoRaisesCorrectPropertyChangedEventTest(PropertyInfo property)
        {
            switch (property.Name)
            {
                case "Id":
                    var e = Assert.Throws<AssertionException>(
                        () => BaseDoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("WrongId"));
                    break;
                case "StringProperty":
                    e = Assert.Throws<AssertionException>(
                        () => BaseDoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("StringProperty"));
                    break;
                case "LongProperty":
                    e = Assert.Throws<AssertionException>(
                        () => BaseDoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("LongProperty"));
                    break;
                default:
                    base.DoRaisesCorrectPropertyChangedEventTest(property);
                    break;
            }
        }
        private void BaseDoRaisesCorrectPropertyChangedEventTest(PropertyInfo property)
        {
            base.DoRaisesCorrectPropertyChangedEventTest(property);
        }

        public class BadFoo : INotifyPropertyChanged
        {
            private int _id;

            public int Id
            {
                get { return _id; }
                set
                {
                    _id = value;
                    OnPropertyChanged("WrongId");
                }
            }

            public string StringProperty { get; set; }

            public long LongProperty
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            protected void OnPropertyChanged(string propertyName)
            {
                var e = PropertyChanged;
                if (e != null) e(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

    }
}
