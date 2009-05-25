class Accumulator {
    private long _minimal = Long.MAX_VALUE;
    private long _maximal = Long.MIN_VALUE;
    private long _count;
    private long _total;

    public synchronized long getMinimal() {
        return _minimal;
    }

    public synchronized long getMaximal() {
        return _maximal;
    }

    public synchronized long getCount() {
        return _count;
    }

    public synchronized long getTotal() {
        return _total;
    }

    public synchronized long getAverage() {
        return _total / _count;
    }

    public synchronized void accumulate(long element) {
        if (element < _minimal) _minimal = element;
        if (element > _maximal) _maximal = element;
        _count++;
        _total += element;
    }
}
