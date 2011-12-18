using System;

namespace ReaderMerge.Readers
{
    /// test reader that returns element in the given array in order. Once it
    /// reaches the end of array, it returns int.MaxValue forever.
    internal class ArrayReader : IReader
    {
        private readonly int[] _source;
        private int _index;

        public ArrayReader(int[] source)
        {
            _source = source;
        }

        public int Read()
        {
            return _source.Length > _index ? _source[_index++] : Int32.MaxValue;
        }
    }
}