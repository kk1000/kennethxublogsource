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
        private const string TitleErrorResyncRevision = "Error Resyncing Source And Destination Revision";
        private const string TitleProcessRevision = "Error Processing Revision";
        private const string TitleErrorProcessingNode = "Error Processing Node";
        private const string ActionModified = "\tModified ";
        private const string ActionCreated = "\tCreated ";
        private const string PropertyKeySourceRevision = "svn2svn:revision";
        private const string PropertyKeyAuthor = "svn:author";
        private const string PropertyKeyDateTime = "svn:date";
        private const int LogBatchSize = 100;

        private static readonly SvnAddArgs _forceAdd = new SvnAddArgs { Force = true };
        private static readonly SvnAddArgs _infiniteForceAdd = new SvnAddArgs{Depth = SvnDepth.Infinity, Force = true};
        private static readonly SvnExportArgs _infiniteOverwriteExport = new SvnExportArgs {Depth = SvnDepth.Infinity, Overwrite = true};
        private static readonly SvnUpdateArgs _ignoreExternalUpdate = new SvnUpdateArgs { IgnoreExternals = true };
        private static readonly SvnLogArgs _oneToHeadLog = new SvnLogArgs(new SvnRevisionRange(SvnRevision.Zero, SvnRevision.Head)) { RetrieveAllProperties = true };
        private static readonly SvnStatusArgs _infiniteStatus = new SvnStatusArgs {Depth = SvnDepth.Infinity, IgnoreExternals = true, RetrieveIgnoredEntries = true};

        private IInteraction _interaction = new NoInteraction();

        private readonly SourceInfo _source;
        private readonly RevisionMap _map = new RevisionMap();
        private readonly SvnClient _svn = new SvnClient();
        private Uri _destination;
        private readonly string _workingDir;

        private volatile bool _stopRequested;
        private long _resyncToRevision;
        private bool _firstLoad = true;

        private bool _ignoreResyncMismatch;
        private bool _ignoreRevisionError;
        private bool _ignoreItemError;


        public Copier(Uri sourceUri, Uri destinationUri, string workingDir)
        {
            _source = new SourceInfo(sourceUri);
            if (destinationUri == null) throw new ArgumentNullException("destinationUri");
            if (workingDir == null) throw new ArgumentNullException("workingDir");
            _destination = destinationUri;
            _workingDir = workingDir;
            CopyAuthor = true;
            CopyDateTime = true;
            CopySourceRevision = true;
        }

        public IInteraction Interaction
        {
            get { return _interaction; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _interaction = value;
            }
        }

        public bool CopyAuthor { get; set; }

        public bool CopyDateTime { get; set; }

        public bool CopySourceRevision { get; set; }

        public void Stop()
        {
            _stopRequested = true;
        }

        public void Copy(long startRevision, long endRevision)
        {
            _stopRequested = false;
            _source.Init(endRevision);
            var destinationDirExists = PrepareDestinationDir();
            if (destinationDirExists) ResyncRevision();
            PrepareWorkingDir();
            NormalizeDestinationUri();
            var lastChange = _source.LastChangeRevision;
            endRevision = endRevision < 0 ? lastChange : Math.Min(endRevision, lastChange);
            var svnLogArgs = new SvnLogArgs
                                 {
                                     End = endRevision,
                                     OperationalRevision = endRevision,
                                     Limit = LogBatchSize,
                                 };
            while (startRevision <= endRevision)
            {
                svnLogArgs.Start = startRevision;
                Collection<SvnLogEventArgs> logEvents;
                _svn.GetLog(_source.Uri, svnLogArgs, out logEvents);
                foreach (var e in logEvents)
                {
                    if (_stopRequested) return;
                    ProcessRevisionLog(e);
                    if (e.Cancel) return;
                    startRevision = e.Revision + 1;
                }
            }
        }

        private void ResyncRevision()
        {
            _svn.Log(_destination, _oneToHeadLog,
                     (s, e) =>
                         {
                             var p = e.RevisionProperties.FirstOrDefault(x=>x.Key ==PropertyKeySourceRevision);
                             if (p == null) return;
                             var sourceRivision = long.Parse(p.StringValue);
                             _map.TrackRevision(sourceRivision, e.Revision);
                         });
            _resyncToRevision = _map.GetLastRevision();
        }

        private bool PrepareDestinationDir()
        {
            Collection<SvnInfoEventArgs> discard;
            var exist = _svn.GetInfo(new SvnUriTarget(_destination), new SvnInfoArgs {ThrowOnError = false}, out discard);
            if (!exist)
            {
                _svn.RemoteCreateDirectory(_destination, new SvnCreateDirectoryArgs {LogMessage = "Migrate from " + _source.Uri});
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
                _svn.CleanUp(_workingDir);
                _svn.Revert(_workingDir, new SvnRevertArgs { Depth = SvnDepth.Infinity });
                _svn.Update(_workingDir, _ignoreExternalUpdate);
                _svn.Status(_workingDir, _infiniteStatus,
                            (s, e) =>
                                { // delete all unversioned files.
                                    if (e.LocalContentStatus == SvnStatus.NotVersioned)
                                    {
                                        string destinationPath = e.Path;
                                        if (File.Exists(destinationPath)) File.Delete(destinationPath);
                                        if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, true);
                                    }
                                });
            }
        }

        private void ProcessRevisionLog(SvnLogEventArgs e)
        {
            var sourceRevision = e.Revision;
            _interaction.Info("{0} {1} {2} {3}", sourceRevision, e.Author, e.Time.ToLocalTime(), e.LogMessage);
            if (sourceRevision <= _resyncToRevision)
            {
                _firstLoad = false;
                if (!_ignoreResyncMismatch)
                    _interaction.DoInteractively(
                        ref _ignoreResyncMismatch, 
                        TitleErrorResyncRevision,
                        () => CheckResyncRevision(sourceRevision));
                return;
            }
            _interaction.DoInteractively(
                ref _ignoreRevisionError,
                TitleProcessRevision,                
                () =>
                    {
                        bool result = _firstLoad
                                          ? ExportDirectory(new SvnUriTarget(_source.Uri, e.Revision), _workingDir)
                                          : ChangeWorkingCopy(e);

                        if (result)
                        {
                            CommitToDestination(e);
                            _firstLoad = false;
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    });
        }


        private bool ExportDirectory(SvnTarget sourceUri, string destinationPath)
        {
            using (var svnClient = new SvnClient())
            {
                svnClient.List(
                    sourceUri,
                    new SvnListArgs {Depth = SvnDepth.Infinity},
                    (s, e) => _interaction.DoInteractively(
                        ref _ignoreItemError,
                        TitleErrorProcessingNode,
                        () => ExportDirectoryListItem(e, sourceUri.Revision, destinationPath)));
            }
            return !_stopRequested;
        }

        private void ExportDirectoryListItem(SvnListEventArgs e, SvnRevision revision, string destinationPath)
        {
            if (_stopRequested)
            {
                e.Cancel = true;
                return;
            }
            destinationPath = Path.Combine(destinationPath, e.Path);
            var source = new SvnUriTarget(e.Uri, revision);
            bool exists;
            if (e.Entry.NodeKind == SvnNodeKind.Directory)
            {
                exists = Directory.Exists(destinationPath);
                if (!exists) Directory.CreateDirectory(destinationPath);
            }
            else
            {
                exists = File.Exists(destinationPath);
                _svn.Export(source, destinationPath, _infiniteOverwriteExport);
            }
            if (destinationPath != _workingDir)
            {
                _svn.Add(destinationPath, _forceAdd);
                _interaction.Trace((exists ? ActionModified : ActionCreated) + destinationPath);
            }
            CopyProperties(source, destinationPath);
        }

        private void CheckResyncRevision(long sourceRevision)
        {
            long destRevision = _map.CheckResyncRevision(sourceRevision);
            if (destRevision > 0)
                _interaction.Trace("\tResync {0} to {1}", sourceRevision, destRevision);
        }

        private void CommitToDestination(SvnLogEventArgs e)
        {
            SvnCommitResult result;
            _svn.Commit(_workingDir, new SvnCommitArgs { LogMessage = e.LogMessage }, out result);
            if (result == null) return;
            var sourceRevision = e.Revision;
            var destinationReivison = result.Revision;
            _map.TrackRevision(sourceRevision, destinationReivison);
            if (CopyAuthor) _svn.SetRevisionProperty(_destination, destinationReivison, PropertyKeyAuthor, e.Author);
            if (CopyDateTime)
                _svn.SetRevisionProperty(_destination, destinationReivison, PropertyKeyDateTime, e.Time.ToString("O") + "Z");
            if (CopySourceRevision)
                _svn.SetRevisionProperty(_destination, destinationReivison, PropertyKeySourceRevision,
                                         sourceRevision.ToString("#0"));
            _svn.Update(_workingDir, _ignoreExternalUpdate);
            _interaction.UpdateProgress(sourceRevision, destinationReivison);
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

            return ProcessNodes(itemsDeleted, e, Delete) &&
                   ProcessNodes(itemsModified, e, Modify) &&
                   ProcessNodes(itemsAdded, e, Add);
        }

        /// <summary>
        /// Return false if _stopRequested, otherwise true.
        /// </summary>
        private bool ProcessNodes(IEnumerable<SvnChangeItem> nodes, SvnLogEventArgs e, Action<SvnChangeItem, SvnLogEventArgs, string> action )
        {
            foreach (var node in nodes)
            {
                if (_stopRequested) return false; // canceled
                var destinationPath = _source.GetDestinationPath(_workingDir, node.Path);
                if (destinationPath == null) continue; // files not in source path.
                _interaction.DoInteractively(
                    ref _ignoreItemError,
                    TitleErrorProcessingNode,
                    () => action(node, e, destinationPath));
            }
            return true; // finished 
        }

        private void Delete(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
            {
                _svn.Delete(destinationPath);
                _interaction.Trace("\tDeleted " + destinationPath);
            }
        }

        private void Modify(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            var source = _source.GetSourceTarget(node, e.Revision);
            if (node.NodeKind == SvnNodeKind.File)
            {
                _svn.Export(source, destinationPath);
            }
            _interaction.Trace(ActionModified + destinationPath);
            CopyProperties(source, destinationPath);
        }

        private void Add(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (destinationPath == _workingDir) return;
            var source = _source.GetSourceTarget(node, e.Revision);
            bool processed = false;
            if (node.CopyFromPath != null)
            {
                processed = TryCopy(node, destinationPath);
                if (!processed && node.NodeKind == SvnNodeKind.Directory)
                {
                    ExportDirectory(source, destinationPath);
                    return;
                }
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
            _interaction.Trace(ActionCreated + destinationPath);
            CopyProperties(source, destinationPath);
        }

        private bool TryCopy(SvnChangeItem node, string destinationPath)
        {
            var relativePath = _source.GetRelativePath(node.CopyFromPath);
            if (relativePath == null) return false;
            var revision = _map.FindDestinationRevision(node.CopyFromRevision);
            if (revision < 0) return false;
            // must use server uri as working copy may have been delete when copy from old revision.
            var copyFromUri = new Uri(_destination, relativePath);
            _svn.Copy(new SvnUriTarget(copyFromUri, revision), destinationPath);
            return true;
        }

        private void CopyProperties(SvnTarget source, string destinationPath)
        {
            Collection<SvnPropertyListEventArgs> props;
            _svn.GetPropertyList(source, out props);
            var keys = new HashSet<string>();
            foreach (var prop in props)
            {
                foreach (var p in prop.Properties)
                {
                    var key = p.Key;
                    keys.Add(key);
                    _svn.SetProperty(destinationPath, key, p.RawValue);
                    _interaction.Trace("\t\tSet {0}=>{1}", key, p.StringValue);
                }
            }
            _svn.GetPropertyList(new SvnPathTarget(destinationPath), out props);
            foreach (var prop in props)
            {
                foreach (var p in prop.Properties)
                {
                    var key = p.Key;
                    if (keys.Contains(key)) continue;
                    _svn.DeleteProperty(destinationPath, key);
                    _interaction.Trace("\t\tDelete {0}", key);
                }
            }
        }
    }
}