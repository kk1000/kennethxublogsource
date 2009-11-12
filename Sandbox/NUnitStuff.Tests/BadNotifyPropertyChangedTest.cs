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
                        () => base.DoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("WrongId"));
                    break;
                case "StringProperty":
                    e = Assert.Throws<AssertionException>(
                        () => base.DoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("StringProperty"));
                    break;
                case "LongProperty":
                    e = Assert.Throws<AssertionException>(
                        () => base.DoRaisesCorrectPropertyChangedEventTest(property));
                    Assert.That(e.Message, Is.StringContaining("LongProperty"));
                    break;
                default:
                    base.DoRaisesCorrectPropertyChangedEventTest(property);
                    break;
            }
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
