using ReaderMerge.Readers;

namespace ReaderMerge.Mergers
{
    internal class BufferReaderMerger : AbstractMerger
    {
        protected override void JustMerge(IReader[] readers, IWriter writer)
        {
            var buffers = ArrayConvert(readers, r => new BufferedReader(r));
            int i;
            while((i=Min(buffers).Read()) != System.Int32.MaxValue)
            {
                writer.Writer(i);
            }
        }

        private static BufferedReader Min(BufferedReader[] readers)
        {
            var min = readers[0];
            foreach (var r in readers)
            {
                if (r.CompareTo(min) < 0) min = r;
            }
            return min;
        }
    }
}