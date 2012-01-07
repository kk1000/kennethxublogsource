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
        private static readonly SvnUpdateArgs _ignoreExternalUpdate = new SvnUpdateArgs { IgnoreExternals = true };
        private static readonly SvnLogArgs _oneToHeadLog = new SvnLogArgs(new SvnRevisionRange(SvnRevision.One, SvnRevision.Head)) { RetrieveAllProperties = true };

        private readonly Dictionary<long, long> _revisionMap = new Dictionary<long, long>();
        private readonly List<long> _revisionHistory = new List<long>();
        private ILog _logger = new NoLog();

        private readonly SvnClient _svn = new SvnClient();
        private readonly Uri _source;
        private Uri _destination;
        private readonly string _workingDir;

        private Uri _sourceRoot;
        private string _sourcePath;
        private volatile bool _stopRequested;
        private long _resyncToRevision;

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
            var destinationDirExists = PrepareDestinationDir();
            if (destinationDirExists) ResyncRevision();
            PrepareWorkingDir();
            NormalizeDestinationUri();
            SeparateSourceRootAndPath();
            new SvnClient().Log(_source, _oneToHeadLog, LogHander);
        }

        private void ResyncRevision()
        {
            _svn.Log(_destination, _oneToHeadLog,
                     (s, e) =>
                         {
                             var p = e.RevisionProperties.FirstOrDefault(x=>x.Key =="svn2svn:revision");
                             if (p == null) return;
                             var sourceRivision = long.Parse(p.StringValue);
                             TrackRevision(sourceRivision, e.Revision);
                         });
            if (_revisionHistory.Count > 0) _resyncToRevision = _revisionHistory[_revisionHistory.Count - 1];
        }

        private bool PrepareDestinationDir()
        {
            Collection<SvnInfoEventArgs> discard;
            var exist = _svn.GetInfo(new SvnUriTarget(_destination), new SvnInfoArgs {ThrowOnError = false}, out discard);
            if (!exist)
            {
                _svn.RemoteCreateDirectory(_destination, new SvnCreateDirectoryArgs {LogMessage = "Migrate from " + _source});
            }
            return exist;
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
            else
            {
                _svn.Revert(_workingDir, new SvnRevertArgs{Depth=SvnDepth.Infinity});
                _svn.Update(_workingDir, _ignoreExternalUpdate);
            }
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
            var sourceRevision = e.Revision;
            _logger.Info("{0} {1} {2} {3}", sourceRevision, e.Author, e.Time.ToLocalTime(), e.LogMessage);
            if (sourceRevision <= _resyncToRevision)
            {
                long destRevision = 0;
                if (sourceRevision >= _revisionHistory[0] &&
                    !_revisionMap.TryGetValue(sourceRevision, out destRevision))
                    throw new InvalidOperationException(
                        "Failed to resync, no matching destination revision for source revision number "
                        + sourceRevision);
                if (destRevision==0)
                    _logger.Trace("\tResync {0} to {1}", sourceRevision, destRevision);
                return;
            }
            try
            {
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
            var sourceRevision = e.Revision;
            var destinationReivison = result.Revision;
            TrackRevision(sourceRevision, destinationReivison);
            if (CopyAuthor) _svn.SetRevisionProperty(_destination, destinationReivison, "svn:author", e.Author);
            if (CopyDateTime)
                _svn.SetRevisionProperty(_destination, destinationReivison, "svn:date", e.Time.ToString("O") + "Z");
            if (CopySourceRevision)
                _svn.SetRevisionProperty(_destination, destinationReivison, "svn2svn:revision",
                                         sourceRevision.ToString("#0"));
            _svn.Update(_workingDir, _ignoreExternalUpdate);
            _logger.UpdateProgress(sourceRevision, destinationReivison);
        }

        private void TrackRevision(long sourceRevision, long destinationReivison)
        {
            _revisionHistory.Add(sourceRevision);
            _revisionMap[sourceRevision] = destinationReivison;
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
            var revision = FindCopyFromRevision(node);
            if (revision < 0) return false;
            // must use server uri as working copy may have been delete when copy from old revision.
            _svn.Copy(new SvnUriTarget(copyFromUri, revision), destinationPath);
            return true;
        }

        private long FindCopyFromRevision(SvnChangeItem node)
        {
            long revision;
            var copyFromRevision = node.CopyFromRevision;
            if (_revisionMap.TryGetValue(copyFromRevision, out revision)) return revision;
            for (int i = _revisionHistory.Count - 1; i >= 0; i--)
            {
                if (_revisionHistory[i] < copyFromRevision) return _revisionMap[_revisionHistory[i]];
            }
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