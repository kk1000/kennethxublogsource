namespace ReaderMerge.Writers
{
    // writer that capatures elements in a buffer array.
    internal class ArrayBufferWriter : IWriter
    {
        public readonly int[] Buffer;
        private int _count;

        public ArrayBufferWriter(int count)
        {
            Buffer = new int[count];
        }

        public void Writer(int i)
        {
            Buffer[_count++] = i;
        }

        public int Count
        {
            get { return _count; }
        }
    }
}