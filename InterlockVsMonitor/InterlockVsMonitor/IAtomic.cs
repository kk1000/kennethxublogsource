namespace InterlockVsMonitor
{
    internal interface IAtomic
    {
        int Value { get; set; }
        int CompareExchange(int newValue, int expected);
        int Exchange(int newValue);
        int Increment();
        int Decrement();
    }
}