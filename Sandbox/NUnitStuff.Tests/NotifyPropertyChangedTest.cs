using System.ComponentModel;
using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// Test cases and exmaple of using <see cref="ValueObjectTestFixture{T}"/>
    /// to test implementation of <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture] public class NotifyPropertyChangedTest 
        : NewableValueObjectTestFixture<NotifyPropertyChangedTest.Foo>
    {
        public class Foo : INotifyPropertyChanged
        {
            private int _id = -1;

            [DefaultValue(-1)]
            public int Id
            {
                get { return _id; }
                set
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }

            private string _stringProperty;
            public string StringProperty
            {
                get { return _stringProperty; }
                set
                {
                    _stringProperty = value;
                    OnPropertyChanged("StringProperty");
                }
            }

            private long _longProperty;
            public long LongProperty
            {
                get { return _longProperty; }
                set
                {
                    _longProperty = value;
                    BoolProperty = !_boolProperty;
                    OnPropertyChanged("LongProperty");
                }
            }

            public bool _boolProperty;
            public bool BoolProperty
            {
                set
                {
                    if (_boolProperty != value)
                    {
                        _boolProperty = value;
                        OnPropertyChanged("BoolProperty");
                    }
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
