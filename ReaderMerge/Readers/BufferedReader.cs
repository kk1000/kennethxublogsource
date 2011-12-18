using System;

namespace ReaderMerge.Readers
{
    internal class BufferedReader : IComparable<BufferedReader>, IReader
    {
        private readonly IReader _reader;
        private int _current;

        public BufferedReader(IReader reader)
        {
            _reader = reader;
            _current = reader.Read();
        }

        public int Read()
        {
            var result = _current;
            _current = _reader.Read();
            return result;
        }

        public int CompareTo(BufferedReader other)
        {
            return _current - other._current;
        }
    }
}