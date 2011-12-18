using System;
using System.Collections.Generic;
using System.Linq;

namespace ReaderMerge.Mergers
{
    abstract class AbstractMerger : IMerger
    {
        public void Merge(IReader[] readers, IWriter writer)
        {
            if (readers == null) throw new ArgumentNullException("readers");
            if (writer == null) throw new ArgumentNullException("writer");
            if (readers.Length == 0) return;
            JustMerge(readers, writer);
        }

        protected abstract void JustMerge(IReader[] readers, IWriter writer);

        // utility method to convert array.
        protected static TResult[] ArrayConvert<TSource, TResult>(TSource[] source, Converter<TSource, TResult> converter)
        {
            var result = new TResult[source.Length];
            int index = 0;
            foreach (var r in source) result[index++] = converter(r);
            return result;
        }

        // utility method that builds a binary tree.
        protected static T BuildBinaryTree<T>(IEnumerable<T> source, Func<T, T, T> twoInOne)
        {
            var buffer = source.ToArray();
            for (var count = buffer.Length; count > 1; count = (count + 1)/2)
            { // Sample count: 19 -> 10 -> 5 -> 3 -> 2 -> 1
                var half = count/2;
                for (int i = 0, j = count - 1; i < half; i++, j--)
                    buffer[i] = twoInOne(buffer[i], buffer[j]);
            }
            return buffer[0];
        }

        //// utility method that builds a binary tree recursively.
        //protected static T BuildBinaryTreeRecursive<T>(IList<T> source, Func<T, T, T> twoInOne)
        //{
        //    return BuildBinaryTreeRecursive(source, 0, source.Count, twoInOne);
        //}

        //// utility method that builds a binary tree recursively.
        //protected static T BuildBinaryTreeRecursive<T>(IList<T> source, int from, int to, Func<T, T, T> twoInOne)
        //{
        //    var size = to - from;
        //    if (size == 1) return source[from];
        //    if (size == 2) return twoInOne(source[from], source[from + 1]);
        //    var mid = (to + from) / 2;
        //    return twoInOne(
        //        BuildBinaryTreeRecursive(source, from, mid, twoInOne),
        //        BuildBinaryTreeRecursive(source, mid, to, twoInOne));
        //}
    }
}