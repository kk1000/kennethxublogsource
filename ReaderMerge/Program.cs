using System;
using System.Diagnostics;
using System.Linq;
using ReaderMerge.Mergers;
using ReaderMerge.Readers;
using ReaderMerge.Writers;

namespace ReaderMerge
{
    class Program
    {
        static readonly Random _random = new Random();

        static int _readerCount = 500; // number of readers used in test
        static int _readerSize = 2000; // number of elements from reader before it returns int.MaxValue.
        static int _count;             // total elements from all readers that are not int.MaxValue.
        static int[][] _arrays;        // data used to generate readers
        static int[] _expected;        // expected out put to writer

        static void Main(string[] args)
        {
            if (args.Length >= 2) _readerSize = int.Parse(args[1]);
            if (args.Length >= 1) _readerCount = int.Parse(args[0]);
            Setup();
            Console.WriteLine("-------- Method ---------+--- Ticks ---+--- ms --");
            TestMerge(new MergeReaderMerger());
            TestMerge(new PriorityQueueMerger());
            TestMerge(new IntBufferMerger());
            TestMerge(new BufferReaderMerger());
            TestMerge(new AdaptiveMerger());
            TestMerge(new BufferReaderLinqMerger());
        }

        private static void Setup()
        {
            _count = _readerCount*_readerSize;
            Console.WriteLine("Using {0} readers, each has {1} elements, total {2}",
                              _readerCount, _readerSize, _count);
            _arrays = new int[_readerCount][];
            _expected = new int[_count];
            int index = 0;
            for (int i = _arrays.Length - 1; i >= 0; i--)
            {
                // array for individual test reader
                var array = GenerateArray(_readerSize, _count * 10);
                Array.Sort(array);
                _arrays[i] = array;
                // add them to expected result
                for (int end = array.Length, j = 0; j < end; j++)
                {
                    _expected[index++] = array[j];
                }
            }
            Array.Sort(_expected);
        }

        private static void TestMerge(IMerger merger)
        {
            var readers = _arrays.Select(x => new ArrayReader(x)).ToArray();
            var writer = new ArrayBufferWriter(_count);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            merger.Merge(readers, writer);
            sw.Stop();
            Console.WriteLine("{0,-25}| {1,12:N0}| {2,8:N0}", merger.GetType().Name, sw.ElapsedTicks, sw.ElapsedMilliseconds);
            Verify(writer.Buffer);
            if (_count <= 100)
                PrintReader(new ArrayReader(writer.Buffer));
        }

        private static void Verify(int[] buffer)
        {
            for (int end = buffer.Length, i = 0; i < end; i++)
            {
                if (buffer[i] != _expected[i])
                {
                    Console.WriteLine("Failed at index {0}, expected {1}, actual {2}.", i, _expected[i], buffer[i]);
                    return;
                }
            }
        }

        private static int[] GenerateArray(int count, int max)
        {
            var result = new int[count];
            for (int i = count - 1; i >= 0; i--)
            {
                result[i] = _random.Next(max);
            }
            return result;
        }

        private static void PrintReader(IReader reader)
        {
            int i;
            while((i=reader.Read()) != int.MaxValue)
                Console.Write(i + " ");
            Console.WriteLine();
        }

        
    }
}
/* Sample run output
L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 10 100000
Using 10 readers, each has 100000 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |      245,192|      104
PriorityQueueMerger      |      514,669|      220
IntBufferMerger          |      156,790|       67
BufferReaderMerger       |      150,934|       64
AdaptiveMerger           |      144,232|       61
BufferReaderLinqMerger   |      750,078|      320

L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 50 20000
Using 50 readers, each has 20000 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |      397,560|      170
PriorityQueueMerger      |      740,588|      316
IntBufferMerger          |      314,682|      134
BufferReaderMerger       |      352,142|      150
AdaptiveMerger           |      317,993|      136
BufferReaderLinqMerger   |    2,718,191|    1,162

L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 80 12500
Using 80 readers, each has 12500 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |      417,954|      178
PriorityQueueMerger      |      788,208|      337
IntBufferMerger          |      414,451|      177
BufferReaderMerger       |      445,433|      190
AdaptiveMerger           |      430,043|      183
BufferReaderLinqMerger   |    4,222,877|    1,806

L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 100 10000
Using 100 readers, each has 10000 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |      456,011|      195
PriorityQueueMerger      |      814,889|      348
IntBufferMerger          |      471,295|      201
BufferReaderMerger       |      518,972|      221
AdaptiveMerger           |      449,396|      192
BufferReaderLinqMerger   |    5,235,117|    2,239

L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 1000 1000
Using 1000 readers, each has 1000 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |      665,469|      284
PriorityQueueMerger      |    1,102,394|      471
IntBufferMerger          |    3,331,601|    1,425
BufferReaderMerger       |    3,340,514|    1,428
AdaptiveMerger           |      661,527|      282
BufferReaderLinqMerger   |   48,684,030|   20,823

L:\OpenSource\ReaderMerge\bin\Release>ReaderMerge.exe 10000 100
Using 10000 readers, each has 100 elements, total 1000000
-------- Method ---------+--- Ticks ---+--- ms --
MergeReaderMerger        |    1,102,314|      471
PriorityQueueMerger      |    1,649,254|      705
IntBufferMerger          |   31,015,068|   13,265
BufferReaderMerger       |   37,418,919|   16,005
AdaptiveMerger           |    1,154,129|      493
BufferReaderLinqMerger   |  497,069,437|  212,605
 */

