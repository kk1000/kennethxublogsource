namespace ReaderMerge
{
    public interface IMerger
    {
        // merge takes a count for testing purpose, the count can be humongous.
        void Merge(IReader[] readers, IWriter writer);
    }
}