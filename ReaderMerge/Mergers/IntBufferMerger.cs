using System.Linq;

namespace ReaderMerge.Mergers
{
    internal class IntBufferMerger : AbstractMerger
    {
        protected override void JustMerge(IReader[] readers, IWriter writer)
        {
            var buffer = ArrayConvert(readers, r => r.Read());
            while (true)
            {
                int index = FindMin(buffer);
                int min = buffer[index];
                if (min == System.Int32.MaxValue) break;
                writer.Writer(min);
                buffer[index] = readers[index].Read();
            }
        }

        private static int FindMin(int[] buffer)
        {
            var index = buffer.Length - 1;
            var min = buffer[index];
            for (int i = index - 1; i >= 0; i--)
            {
                int x = buffer[i];
                if (x < min)
                {
                    min = x;
                    index = i;
                }
            }
            return index;
        }
    }
}