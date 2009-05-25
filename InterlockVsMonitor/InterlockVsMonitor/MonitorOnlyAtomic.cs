namespace InterlockVsMonitor
{
    internal class MonitorOnlyAtomic : IAtomic
    {

        private int _value;

        public virtual int Value
        {
            get { lock (this) return _value; }
            set { lock (this) _value = value; }
        }

        public virtual int CompareExchange(int newValue, int expected)
        {
            lock (this)
            {
                int orig = _value;
                if (expected == orig) _value = newValue;
                return orig;
            }
        }

        public virtual int Exchange(int newValue)
        {
            lock (this)
            {
                int orig = _value;
                _value = newValue;
                return orig;
            }
        }

        public virtual int Increment()
        {
            lock (this)
            {
                int value = _value;
                _value = ++value;
                return value;
            }
        }

        public virtual int Decrement()
        {
            lock (this)
            {
                int value = _value;
                _value = --value;
                return value;
            }
        }
    }
}