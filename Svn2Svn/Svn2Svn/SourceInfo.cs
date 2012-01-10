using System;
using System.IO;
using System.Text;
using SharpSvn;

namespace Svn2Svn
{
    public class SourceInfo
    {
        private readonly Uri _source;
        private Uri _sourceRoot;
        private string _sourcePath;
        private long _lastChangeRevision;

        public SourceInfo(Uri sourceUri)
        {
            if (sourceUri == null) throw new ArgumentNullException("sourceUri");
            _source = sourceUri;
        }

        public void Init(long revision)
        {
            var info = GetSourceInfo(revision);
            _lastChangeRevision = info.LastChangeRevision;
            SeparateSourceRootAndPath(info, out _sourceRoot, out _sourcePath);
        }

        public long LastChangeRevision
        {
            get { return _lastChangeRevision; }
        }

        public Uri Uri
        {
            get { return _source; }
        }

        private SvnInfoEventArgs GetSourceInfo(long revision)
        {
            SvnInfoEventArgs info;
            var sourceTarget = new SvnUriTarget(_source, revision == -1 ? SvnRevision.Head : revision);
            using (var svn = new SvnClient()) svn.GetInfo(sourceTarget, out info);
            return info;
        }

        private static void SeparateSourceRootAndPath(SvnInfoEventArgs info, out Uri sourceRoot, out string sourcePath)
        {
            sourceRoot = info.RepositoryRoot;
            string s = info.Uri.ToString().Substring(sourceRoot.ToString().Length);
            var length = s.Length;
            var sb = new StringBuilder(length + 1);
            if (length == 0 || s[0] != '/') sb.Append('/');
            sb.Append(s);
            if (length != 0 && s[length - 1] == '/') sb.Length -= 1;
            sourcePath = sb.ToString();
        }

        public string GetDestinationPath(string workingDir, string nodePath)
        {
            var relativePath = GetRelativePath(nodePath);
            return relativePath == null ? null : Path.Combine(workingDir, relativePath);
        }

        public string GetRelativePath(string path)
        {
            // e.g. _sourePath = "/abc"
            if (!path.StartsWith(_sourcePath)) return null; // path: "/notabc"
            var sourcePathLenth = _sourcePath.Length;
            if (path.Length == sourcePathLenth) return string.Empty; // path: "/abc"
            if (sourcePathLenth > 1 && path[sourcePathLenth] != '/') return null; // path: "/abcandmore"
            return path.Substring(sourcePathLenth + 1); // path: "/abc/rest/of/uri"
        }

        public SvnUriTarget GetSourceTarget(SvnChangeItem node, SvnRevision revision)
        {
            return new SvnUriTarget(new Uri(_sourceRoot, node.Path.Substring(1)), revision);
        }
    }
}