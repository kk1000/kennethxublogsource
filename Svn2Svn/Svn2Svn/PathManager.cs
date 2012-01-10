using System;
using System.IO;
using System.Text;
using SharpSvn;

namespace Svn2Svn
{
    public class PathManager
    {
        private readonly Uri _source;
        private readonly Uri _sourceRoot;
        private readonly string _sourcePath;
        private readonly string _workingDir;

        public PathManager(Uri sourceUri, string workingDir)
        {
            if (sourceUri == null) throw new ArgumentNullException("sourceUri");
            if (workingDir == null) throw new ArgumentNullException("workingDir");
            _source = sourceUri;
            _workingDir = workingDir;
            SeparateSourceRootAndPath(out _sourceRoot, out _sourcePath);
        }

        public string WorkingDir
        {
            get { return _workingDir; }
        }

        public Uri SourceUri
        {
            get { return _source; }
        }

        //public Uri SourceRoot
        //{
        //    get { return _sourceRoot; }
        //}

        //public string SourcePath
        //{
        //    get { return _sourcePath; }
        //}

        private void SeparateSourceRootAndPath(out Uri sourceRoot, out string sourcePath)
        {
            SvnInfoEventArgs info;
            using (var svn = new SvnClient())
                svn.GetInfo(new SvnUriTarget(_source), out info);
            sourceRoot = info.RepositoryRoot;
            string s = info.Uri.ToString().Substring(sourceRoot.ToString().Length);
            var length = s.Length;
            var sb = new StringBuilder(length + 1);
            if (length == 0 || s[0] != '/') sb.Append('/');
            sb.Append(s);
            if (length != 0 && s[length - 1] == '/') sb.Length -= 1;
            sourcePath = sb.ToString();
        }

        public string GetDestinationPath(string nodePath)
        {
            var relativePath = GetRelativePath(nodePath);
            return relativePath == null ? null : Path.Combine(_workingDir, relativePath);
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