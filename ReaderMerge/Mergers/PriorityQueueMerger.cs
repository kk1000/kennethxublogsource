using System.Linq;
using ReaderMerge.Readers;
using Spring.Collections.Generic;

namespace ReaderMerge.Mergers
{
    internal class PriorityQueueMerger : AbstractMerger
    {
        protected override void JustMerge(IReader[] readers, IWriter writer)
        {
            var queue = new PriorityQueue<BufferedReader>(readers.Select(x => new BufferedReader(x)));
            while (true)
            {
                var b = queue.Remove();
                int i = b.Read();
                if (i == System.Int32.MaxValue) break;
                writer.Writer(i);
                queue.Add(b);
            }
        }
    }
}