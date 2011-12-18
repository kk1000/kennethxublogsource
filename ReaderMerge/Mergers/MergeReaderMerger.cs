using ReaderMerge.Readers;
using System;

namespace ReaderMerge.Mergers
{
    internal class MergeReaderMerger : AbstractMerger
    {
        protected override void JustMerge(IReader[] readers, IWriter writer)
        {
            var reader = BuildBinaryTree(readers, (r1, r2) => new MergeReader(r1, r2));
            int i; while ((i = reader.Read()) < Int32.MaxValue) writer.Writer(i);
        }
    }
}