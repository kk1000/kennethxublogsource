
public class Accumulator {
    private long _minimal = Long.MAX_VALUE;
    public synchronized long getMinimal()
    {
        return _minimal;
    }

    private long _maximal = Long.MIN_VALUE;
    public synchronized long getMaximal()
    {
        return _maximal;
    }

    private long _count;
    public synchronized long getCount()
    {
        return _count;
    }

    private long _total;
    public synchronized long getTotal()
    {
        return _total;
    }

    public synchronized long getAverage()
    {
        return _total / _count;
    }

    public synchronized void Accumulate(long element)
    {
        if (element < _minimal) _minimal = element;
        if (element > _maximal) _maximal = element;
        _count++;
        _total += element;
    }
}
