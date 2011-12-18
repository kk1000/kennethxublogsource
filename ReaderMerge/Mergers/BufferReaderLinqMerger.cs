using System.Linq;
using ReaderMerge.Readers;

namespace ReaderMerge.Mergers
{
    internal class BufferReaderLinqMerger : AbstractMerger
    {
        protected override void JustMerge(IReader[] readers, IWriter writer)
        {
            var buffers = ArrayConvert(readers, r => new BufferedReader(r));
            int i;
            // Linq Min is very slow due to IEnumerable.
            while ((i=buffers.Min().Read()) != System.Int32.MaxValue)
            {
                writer.Writer(i); 
            }
        }
    }
}