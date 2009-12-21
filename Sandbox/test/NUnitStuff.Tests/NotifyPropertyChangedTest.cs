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
