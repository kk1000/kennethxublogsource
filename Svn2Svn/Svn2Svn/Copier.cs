#region License

/*
 * Copyright (C) 2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using SharpSvn;

namespace Svn2Svn
{
    /// <summary>
    /// Does the actual svn copy work.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class Copier
    {
        private static readonly SvnAddArgs _infiniteForceAdd = new SvnAddArgs{Depth = SvnDepth.Infinity, Force = true};
        private static readonly SvnExportArgs _infiniteOverwriteExport = new SvnExportArgs {Depth = SvnDepth.Infinity, Overwrite = true};

        private readonly Dictionary<long, long> _revisionMap = new Dictionary<long, long>();
        private ILog _logger = new NoLog();

        private readonly SvnClient _svn = new SvnClient();
        private readonly Uri _source;
        private Uri _destination;
        private readonly string _workingDir;

        private Uri _sourceRoot;
        private string _sourcePath;
        private volatile bool _stopRequested;

        public Copier(Uri sourceUri, Uri destinationUri, string workingDir)
        {
            if (sourceUri == null) throw new ArgumentNullException("sourceUri");
            if (destinationUri == null) throw new ArgumentNullException("destinationUri");
            if (workingDir == null) throw new ArgumentNullException("workingDir");
            _source = sourceUri;
            _destination = destinationUri;
            _workingDir = workingDir;
            CopyAuthor = true;
            CopyDateTime = true;
            CopySourceRevision = true;
        }

        public ILog Logger
        {
            get { return _logger; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _logger = value;
            }
        }

        public bool CopyAuthor { get; set; }

        public bool CopyDateTime { get; set; }

        public bool CopySourceRevision { get; set; }

        public void Stop()
        {
            _stopRequested = true;
        }

        public void Copy()
        {
            _stopRequested = false;
            PrepareDestinationDir();
            PrepareWorkingDir();
            NormalizeDestinationUri();
            SeparateSourceRootAndPath();
            var svnLogArgs = new SvnLogArgs(new SvnRevisionRange(SvnRevision.One, SvnRevision.Head));
            new SvnClient().Log(_source, svnLogArgs, LogHander);
        }

        private void PrepareDestinationDir()
        {
            Collection<SvnInfoEventArgs> discard;
            if (!_svn.GetInfo(new SvnUriTarget(_destination), new SvnInfoArgs {ThrowOnError = false}, out discard))
            {
                _svn.RemoteCreateDirectory(_destination, new SvnCreateDirectoryArgs {LogMessage = "Migrate from " + _source});
            }
        }

        private void NormalizeDestinationUri()
        {
            SvnInfoEventArgs info;
            _svn.GetInfo(new SvnPathTarget(_workingDir), out info);
            if (!info.Uri.ToString().Equals(_destination.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Working dir does not match with destination repo: " +
                                                    info.Uri + " vs. " + _destination);
            }
            _destination = info.Uri;
        }

        private void PrepareWorkingDir()
        {
            var workingDir = new DirectoryInfo(_workingDir);
            if (!workingDir.Exists)
                _svn.CheckOut(new SvnUriTarget(_destination), _workingDir);
        }

        private void SeparateSourceRootAndPath()
        {
            SvnInfoEventArgs info;
            _svn.GetInfo(new SvnUriTarget(_source), out info);
            _sourceRoot = info.RepositoryRoot;
            _sourcePath = info.Uri.ToString().Substring(_sourceRoot.ToString().Length);
        }

        private void LogHander(object sender, SvnLogEventArgs e)
        {
            if (_stopRequested)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                _logger.Info("{0} {1} {2} {3}", e.Revision, e.Author, e.Time.ToLocalTime(), e.LogMessage);

                if (ChangeWorkingCopy(e)) CommitToDestination(e);
                else e.Cancel = true;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
        }

        private void CommitToDestination(SvnLogEventArgs e)
        {
            SvnCommitResult result;
            _svn.Commit(_workingDir, new SvnCommitArgs {LogMessage = e.LogMessage}, out result);
            if (result == null) return;
            _revisionMap[e.Revision] = result.Revision;
            if (CopyAuthor) _svn.SetRevisionProperty(_destination, result.Revision, "svn:author", e.Author);
            if (CopyDateTime)
                _svn.SetRevisionProperty(_destination, result.Revision, "svn:date", e.Time.ToString("O") + "Z");
            if (CopySourceRevision)
                _svn.SetRevisionProperty(_destination, result.Revision, "svn2svn:revision",
                                         e.Revision.ToString("#0"));
            _svn.Update(_workingDir, new SvnUpdateArgs { IgnoreExternals = true });
            _logger.UpdateProgress(e.Revision, result.Revision);
        }

        /// <summary>
        /// Return false we didn't finish because _stopRequested, otherwise true.
        /// </summary>
        private bool ChangeWorkingCopy(SvnLogEventArgs e)
        {
            var itemsAdded = from x in e.ChangedPaths 
                             where x.Action == SvnChangeAction.Add 
                             orderby x.Path 
                             select x;

            var itemsModified = from x in e.ChangedPaths 
                                where x.Action == SvnChangeAction.Modify 
                                select x;

            var itemsDeleted = from x in e.ChangedPaths
                               where x.Action == SvnChangeAction.Delete
                               orderby x.Path descending
                               select x;

            return ProcessNodes(itemsAdded, e, Add) &&
                   ProcessNodes(itemsModified, e, Modify) &&
                   ProcessNodes(itemsDeleted, e, Delete);
        }

        /// <summary>
        /// Return false if _stopRequested, otherwise true.
        /// </summary>
        private bool ProcessNodes(IEnumerable<SvnChangeItem> nodes, SvnLogEventArgs e, Action<SvnChangeItem, SvnLogEventArgs, string> action )
        {
            foreach (var node in nodes)
            {
                if (_stopRequested) return false; // canceled
                var destinationPath = GetDestinationPath(node);
                if (destinationPath == null) continue; // files not in source path.
                action(node, e, destinationPath);
            }
            return true; // finished 
        }

        private void Delete(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
            {
                _svn.Delete(destinationPath);
                _logger.Trace("\tDeleted " + destinationPath);
            }
        }

        private void Modify(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            var source = GetSourceTarget(node, e);
            if (node.NodeKind == SvnNodeKind.File)
            {
                _svn.Export(source, destinationPath);
            }
            _logger.Trace("\tModified " + destinationPath);
            CopyProperties(source, destinationPath);
        }

        private void Add(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (destinationPath == _workingDir) return;
            var source = GetSourceTarget(node, e);
            bool processed = false;
            if (node.CopyFromPath != null)
            {
                processed = TryCopy(node, destinationPath);
            }
            else if (node.NodeKind == SvnNodeKind.Directory)
            {
                _svn.CreateDirectory(destinationPath);
                processed = true;
            }
            if (!processed)
            {
                _svn.Export(source, destinationPath, _infiniteOverwriteExport);
                _svn.Add(destinationPath, _infiniteForceAdd);
            }
            _logger.Trace("\tCreated " + destinationPath);
            CopyProperties(source, destinationPath);
        }

        private bool TryCopy(SvnChangeItem node, string destinationPath)
        {
            string copyFrom = node.CopyFromPath.Substring(1);
            if (_sourcePath.Length != 0 && !copyFrom.StartsWith(_sourcePath)) return false;

            var copyFromUri = new Uri(_destination, copyFrom.Substring(_sourcePath.Length));
            var revision = FindCopyFromRevision(node, copyFrom);
            if (revision < 0) return false;
            // must use server uri as working copy may have been delete when copy from old revision.
            _svn.Copy(new SvnUriTarget(copyFromUri, revision), destinationPath);
            return true;
        }

        private long FindCopyFromRevision(SvnChangeItem node, string copyFrom)
        {
            long revision;
            if (_revisionMap.TryGetValue(node.CopyFromRevision, out revision)) return revision;
            SvnInfoEventArgs info;
            _svn.GetInfo(new SvnUriTarget(new Uri(_sourceRoot, copyFrom), node.CopyFromRevision), out info);
            if (_revisionMap.TryGetValue(info.LastChangeRevision, out revision)) return revision;
            return -1;
        }

        private void CopyProperties(SvnTarget source, string destinationPath)
        {
            Collection<SvnPropertyListEventArgs> props;
            _svn.GetPropertyList(source, out props);
            foreach (var prop in props)
            {
                foreach (var p in prop.Properties)
                {
                    _svn.SetProperty(destinationPath, p.Key, p.RawValue);
                    _logger.Trace("\t\tSet {0}=>{1}", p.Key, p.StringValue);
                }
            }
        }

        private string GetDestinationPath(SvnChangeItem node)
        {
            var nodePath = node.RepositoryPath.ToString();
            return nodePath.StartsWith(_sourcePath)
                       ? Path.Combine(_workingDir, nodePath.Substring(_sourcePath.Length))
                       : null;
        }

        private SvnUriTarget GetSourceTarget(SvnChangeItem node, SvnLogEventArgs e)
        {
            return new SvnUriTarget(new Uri(_sourceRoot, node.RepositoryPath.ToString()), e.Revision);
        }
    }
}