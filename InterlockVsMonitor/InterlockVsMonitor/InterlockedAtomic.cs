using System.Threading;

namespace InterlockVsMonitor
{
    internal class InterlockedAtomic : IAtomic
    {
        private volatile int _value;

        public virtual int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual int CompareExchange(int newValue, int expected)
        {
            return Interlocked.CompareExchange(ref _value, newValue, expected);
        }

        public virtual int Exchange(int newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
        }

        public virtual int Increment()
        {
            return Interlocked.Increment(ref _value);
        }

        public virtual int Decrement()
        {
            return Interlocked.Decrement(ref _value);
        }
    }
}