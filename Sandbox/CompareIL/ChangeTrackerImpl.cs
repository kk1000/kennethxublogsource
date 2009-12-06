using System;
using CodeSharp;
using CodeSharp.Emit;

namespace CompareIL
{
    internal class ChangeTrackerForIValueObject : ChangeTrackerBase, IValueObject, ICompositeProxy<IValueObject>
    {
        private IValueObject _wrapped;
        private ChangeTrackerForIValueComponent _ComponentProperty;

        public ChangeTrackerForIValueObject(IValueObject wrapped)
        {
            _wrapped = wrapped;
        }

        public string SimpleProperty
        {
            get { return _wrapped.SimpleProperty; }
            set
            {
                if (_wrapped.SimpleProperty != value)
                {
                    _wrapped.SimpleProperty = value;
                    FirePropertyChanged("SimpleProperty");
                }
            }
        }

        public bool IsReadonly { get; set; }

        public int IntProperty
        {
            get { return _wrapped.IntProperty; }
            set
            {
                if (_wrapped.IntProperty != value)
                {
                    _wrapped.IntProperty = value;
                    FirePropertyChanged("IntProperty");
                }
            }
        }

        public int LongProperty
        {
            get { return _wrapped.LongProperty; }
            set
            {
                if(_wrapped.LongProperty != value)
                {
                    _wrapped.LongProperty = value;
                    FirePropertyChanged("LongProperty");
                }
            }
        }

        public object ObjectProperty
        {
            get { return _wrapped.ObjectProperty; }
            set
            {
                if (_wrapped.ObjectProperty != value)
                {
                    _wrapped.ObjectProperty = value;
                    FirePropertyChanged("ObjectProperty");
                }
            }
        }

        public IValueComponent ComponentProperty
        {
            get
            {
                IValueComponent p = _wrapped.ComponentProperty;
                if (p==null)
                {
                    _ComponentProperty = null;
                    return null;
                }
                if (_ComponentProperty == null || !ReferenceEquals(_ComponentProperty._wrapped, p))
                {
                    _ComponentProperty = new ChangeTrackerForIValueComponent(p);
                }
                return _ComponentProperty;
            }
            set 
            {
                if(ReferenceEquals(value, _ComponentProperty)) return;
                if(ReferenceEquals(value, _ComponentProperty._wrapped)) return;
                var proxy = value as ChangeTrackerForIValueComponent;
                if (proxy == null)
                {
                    _wrapped.ComponentProperty = value;
                }
                else
                {
                    _wrapped.ComponentProperty = proxy._wrapped;
                    _ComponentProperty = proxy;
                }
                FirePropertyChanged("ComponentProperty");
            }
        }

        public bool AllGood()
        {
            return !IsReadonly && _wrapped.IntProperty == 123 || _wrapped.SimpleProperty == "any";
        }

        public IValueObject Target
        {
            get { return _wrapped; }
        }
    }

    internal class ChangeTrackerForIValueComponent : ChangeTrackerBase, IValueComponent
    {
        internal readonly IValueComponent _wrapped;

        public ChangeTrackerForIValueComponent(IValueComponent wrapped)
        {
            _wrapped = wrapped;
        }

        public string StringProperty
        {
            get { return _wrapped.StringProperty; }
            set
            {
                if(_wrapped.StringProperty != value)
                {
                    _wrapped.StringProperty = value;
                    FirePropertyChanged("StringProperty");
                }
            }
        }

        public object ObjectProperty
        {
            get { return _wrapped.ObjectProperty; }
            set
            {
                if (_wrapped.ObjectProperty != value)
                {
                    _wrapped.ObjectProperty = value;
                    FirePropertyChanged("ObjectProperty");
                }
            }
        }
    }
}
