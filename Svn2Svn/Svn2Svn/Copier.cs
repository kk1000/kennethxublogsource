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
        private const int LogBatchSize = 100;

        private static readonly SvnUpdateArgs _ignoreExternalUpdate = new SvnUpdateArgs { IgnoreExternals = true };
        private static readonly SvnStatusArgs _infiniteStatus = new SvnStatusArgs {Depth = SvnDepth.Infinity, IgnoreExternals = true, RetrieveIgnoredEntries = true};

        private readonly Global _g = new Global();
        private RevisionProcessor _n;

        public Copier(Uri sourceUri, Uri destinationUri, string workingDir)
        {
            _g.Source = new SourceInfo(sourceUri);
            if (destinationUri == null) throw new ArgumentNullException("destinationUri");
            if (workingDir == null) throw new ArgumentNullException("workingDir");
            _g.Destination = destinationUri;
            _g.WorkingDir = workingDir;
            CopyAuthor = true;
            CopyDateTime = true;
            CopySourceRevision = true;
        }

        public IInteraction Interaction
        {
            get { return _g.Interaction; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _g.Interaction = value;
            }
        }

        public bool CopyAuthor { get; set; }

        public bool CopyDateTime { get; set; }

        public bool CopySourceRevision { get; set; }

        public void Stop()
        {
            _g.StopRequested = true;
        }

        public void Copy(long startRevision, long endRevision)
        {
            _g.StopRequested = false;
            _g.Source.Init(endRevision);
            _n = new RevisionProcessor(_g)
                     {
                         CopyAuthor = CopyAuthor,
                         CopyDateTime = CopyDateTime,
                         CopySourceRevision = CopySourceRevision,
                     };
            var destinationDirExists = PrepareDestinationDir();
            if (destinationDirExists) _n.ResyncRevision();
            PrepareWorkingDir();
            NormalizeDestinationUri();
            var lastChange = _g.Source.LastChangeRevision;
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
                _g.Svn.GetLog(_g.Source.Uri, svnLogArgs, out logEvents);
                foreach (var e in logEvents)
                {
                    if (_g.StopRequested) return;
                    _n.ProcessRevisionLog(e);
                    if (e.Cancel) return;
                    startRevision = e.Revision + 1;
                }
            }
        }

        private bool PrepareDestinationDir()
        {
            Collection<SvnInfoEventArgs> discard;
            var exist = _g.Svn.GetInfo(new SvnUriTarget(_g.Destination), new SvnInfoArgs {ThrowOnError = false}, out discard);
            if (!exist)
            {
                _g.Svn.RemoteCreateDirectory(_g.Destination, 
                    new SvnCreateDirectoryArgs {LogMessage = "Migrate from " + _g.Source.Uri, CreateParents = true});
            }
            return exist;
        }

        private void NormalizeDestinationUri()
        {
            var d = _g.Destination.ToString();
            if (d[d.Length - 1] != '/') d += '/';

            SvnInfoEventArgs info;
            _g.Svn.GetInfo(new SvnPathTarget(_g.WorkingDir), out info);
            if (!info.Uri.ToString().Equals(d, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Working dir does not match with destination repo: " +
                                                    info.Uri + " vs. " + _g.Destination);
            }
            _g.Destination = info.Uri;
        }

        private void PrepareWorkingDir()
        {
            var workingDir = new DirectoryInfo(_g.WorkingDir);
            if (!workingDir.Exists)
            {
                _g.Svn.CheckOut(new SvnUriTarget(_g.Destination), _g.WorkingDir);
                return;
            }
            _g.Svn.CleanUp(_g.WorkingDir);
            Collection<SvnStatusEventArgs> statuses;
            _g.Svn.GetStatus(_g.WorkingDir, _infiniteStatus, out statuses);
            var infiniteRevert = new SvnRevertArgs {Depth = SvnDepth.Infinity};
            foreach (var e in from x in statuses orderby x.Path descending select x)
            {
                switch (e.LocalContentStatus)
                {
                    case SvnStatus.Added:
                        _g.Svn.Revert(e.Path, infiniteRevert);
                        string destinationPath = e.Path;
                        if (File.Exists(destinationPath)) File.Delete(destinationPath);
                        if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, true);
                        break;
                    case SvnStatus.NotVersioned:
                        DeleteFromFileSystem(e);
                        break;
                    default:
                        _g.Svn.Revert(e.Path);
                        break;
                }
            }
            _g.Svn.Update(_g.WorkingDir, _ignoreExternalUpdate);
        }

        private static void DeleteFromFileSystem(SvnStatusEventArgs e)
        {
            if (e.NodeKind == SvnNodeKind.Directory) Directory.Delete(e.Path, true);
            else File.Delete(e.Path);
        }
    }
}