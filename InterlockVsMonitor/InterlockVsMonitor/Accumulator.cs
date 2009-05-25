namespace InterlockVsMonitor
{
    internal class Accumulator
    {
        private long _minimal = long.MaxValue;
        private long _maximal = long.MinValue;
        private long _count;
        private long _total;

        public long Minimal
        {
            get { lock (this) return _minimal; }
        }

        public long Maximal
        {
            get { lock (this) return _maximal; }
        }

        public long Count
        {
            get { lock (this) return _count; }
        }

        public long Total
        {
            get { lock (this) return _total; }
        }

        public long Average
        {
            get { lock (this) return _total / _count; }
        }

        public void Accumulate(long element)
        {
            lock (this)
            {
                if (element < _minimal) _minimal = element;
                if (element > _maximal) _maximal = element;
                _count++;
                _total += element;
            }
        }
    }
}