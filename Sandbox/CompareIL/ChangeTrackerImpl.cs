using System.Collections.Generic;
using CodeSharp;
using CodeSharp.Emit;
using CodeSharp.Proxy;

namespace CompareIL
{
    internal class ProxyIValueObject : ChangeTrackerBase, IValueObject
    {
        private readonly IValueObject _target;
        // ReSharper disable InconsistentNaming
        private IValueComponent _ComponentProperty;
        private IList<IValueComponent> _ComponentList;
        private IDictionary<int, IValueComponent> _ComponentDictionary;
        // ReSharper restore InconsistentNaming

        private ProxyIValueObject(IValueObject target)
        {
            _target = target;
        }

        public static IValueObject NewProxy(IValueObject target)
        {
            return target == null ? null : new ProxyIValueObject(target);
        }

        public static IValueObject GetTarget(IValueObject proxy)
        {
            var confirmProxy = proxy as ProxyIValueObject;
            return confirmProxy != null ? confirmProxy._target : proxy;
        }

        public string SimpleProperty
        {
            get { return _target.SimpleProperty; }
            set
            {
                if (_target.SimpleProperty != value)
                {
                    _target.SimpleProperty = value;
                    FirePropertyChanged("SimpleProperty");
                }
            }
        }

        public bool IsReadonly { get; set; }

        public int IntProperty
        {
            get { return _target.IntProperty; }
            set
            {
                if (_target.IntProperty != value)
                {
                    _target.IntProperty = value;
                    FirePropertyChanged("IntProperty");
                }
            }
        }

        public int LongProperty
        {
            get { return _target.LongProperty; }
            set
            {
                if(_target.LongProperty != value)
                {
                    _target.LongProperty = value;
                    FirePropertyChanged("LongProperty");
                }
            }
        }

        public object ObjectProperty
        {
            get { return _target.ObjectProperty; }
            set
            {
                if (_target.ObjectProperty != value)
                {
                    _target.ObjectProperty = value;
                    FirePropertyChanged("ObjectProperty");
                }
            }
        }

        public IValueComponent ComponentProperty
        {
            get
            {
                IValueComponent component = _target.ComponentProperty;
                if (component==null)
                {
                    _ComponentProperty = null;
                    return null;
                }
                if (_ComponentProperty == null || !ReferenceEquals(NotifyPropertyChangedFactory.GetTarget(_ComponentProperty), component))
                {
                    _ComponentProperty = NotifyPropertyChangedFactory.GetProxy(component);
                }
                return _ComponentProperty;
            }
            set 
            {
                IValueComponent newTarget = NotifyPropertyChangedFactory.GetTarget(value);
                if (ReferenceEquals(_target.ComponentProperty, newTarget)) return;

                _target.ComponentProperty = newTarget;
                _ComponentProperty = NotifyPropertyChangedFactory.GetProxy(value);
                FirePropertyChanged("ComponentProperty");
            }
        }

        public IList<IValueComponent> ComponentList
        {
            get
            {
                IList<IValueComponent> component = _target.ComponentList;
                if (component == null)
                {
                    _ComponentList = null;
                    return null;
                }
                if (_ComponentList == null || !ReferenceEquals(NotifyPropertyChangedFactory.GetTarget(_ComponentList), component))
                {
                    _ComponentList = NotifyPropertyChangedFactory.GetProxy(component);
                }
                return _ComponentList;
            }
            set
            {
                IList<IValueComponent> newTarget = NotifyPropertyChangedFactory.GetTarget(value);
                if (ReferenceEquals(_target.ComponentList, newTarget)) return;

                _target.ComponentList = newTarget;
                _ComponentList = NotifyPropertyChangedFactory.GetProxy(value);
                FirePropertyChanged("ComponentList");
            }
        }

        public IDictionary<int, IValueComponent> ComponentDictionary
        {
            get
            {
                IDictionary<int, IValueComponent> component = _target.ComponentDictionary;
                if (component == null)
                {
                    _ComponentDictionary = null;
                    return null;
                }
                if (_ComponentDictionary == null || !ReferenceEquals(NotifyPropertyChangedFactory.GetTarget(_ComponentDictionary), component))
                {
                    _ComponentDictionary = NotifyPropertyChangedFactory.GetProxy(component);
                }
                return _ComponentDictionary;
            }
            set
            {
                IDictionary<int, IValueComponent> newTarget = NotifyPropertyChangedFactory.GetTarget(value);
                if (ReferenceEquals(_target.ComponentDictionary, newTarget)) return;

                _target.ComponentDictionary = newTarget;
                _ComponentDictionary = NotifyPropertyChangedFactory.GetProxy(value);
                FirePropertyChanged("ComponentDictionary");
            }

        }

        public bool AllGood()
        {
            return !IsReadonly && _target.IntProperty == 123 || _target.SimpleProperty == "any";
        }

        public IValueObject Target
        {
            get { return _target; }
        }
    }

    internal class ProxyIValueComponent : ChangeTrackerBase, IValueComponent
    {
        internal readonly IValueComponent _target;

        public ProxyIValueComponent(IValueComponent target)
        {
            _target = target;
        }

        public string StringProperty
        {
            get { return _target.StringProperty; }
            set
            {
                if(_target.StringProperty != value)
                {
                    _target.StringProperty = value;
                    FirePropertyChanged("StringProperty");
                }
            }
        }

        public object ObjectProperty
        {
            get { return _target.ObjectProperty; }
            set
            {
                if (_target.ObjectProperty != value)
                {
                    _target.ObjectProperty = value;
                    FirePropertyChanged("ObjectProperty");
                }
            }
        }
    }
}
