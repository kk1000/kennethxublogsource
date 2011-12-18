namespace ReaderMerge
{
    public interface IMerger
    {
        // For testing purpose, the process can stop when it reaches Int32.MaxValue.
        void Merge(IReader[] readers, IWriter writer);
    }
}