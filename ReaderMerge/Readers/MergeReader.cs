namespace ReaderMerge.Readers
{
    internal class MergeReader : IReader
    {
        private readonly IReader[] _reader;
        private readonly int[] _current;

        public MergeReader(IReader r1, IReader r2)
        {
            _reader = new[] {r1, r2};
            _current = new[] {r1.Read(), r2.Read()};
        }

        public int Read()
        {
            var i = _current[0] < _current[1] ? 0 : 1;
            var result = _current[i];
            _current[i] = _reader[i].Read();
            return result;
        }
    }
}