namespace ReaderMerge.Mergers
{
    internal class AdaptiveMerger : IMerger
    {
        public void Merge(IReader[] readers, IWriter writer)
        {
            var merger = readers.Length >= 80 ? (IMerger) new MergeReaderMerger() : new IntBufferMerger();
            merger.Merge(readers, writer);
        }
    }
}