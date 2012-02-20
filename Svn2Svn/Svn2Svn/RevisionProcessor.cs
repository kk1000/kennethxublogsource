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
using System.Linq;
using SharpSvn;

namespace Svn2Svn
{
    /// <summary>
    /// Logic to copy a revision.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class RevisionProcessor
    {
        private const string TitleErrorResyncRevision = "Error Resyncing Source And Destination Revision";
        private const string TitleProcessRevision = "Error Processing Revision";
        private const string PropertyKeySourceRevision = "svn2svn:revision";
        private const string PropertyKeyAuthor = "svn:author";
        private const string PropertyKeyDateTime = "svn:date";

        private static readonly SvnUpdateArgs _ignoreExternalUpdate = new SvnUpdateArgs { IgnoreExternals = true };
        private static readonly SvnLogArgs _oneToHeadLog = new SvnLogArgs(new SvnRevisionRange(SvnRevision.Zero, SvnRevision.Head)) { RetrieveAllProperties = true };

        private readonly Global _g = new Global();
        private readonly NodeProcessor _nodeProcessor;

        private long _resyncToRevision;
        private bool _firstLoad = true;

        private bool _ignoreResyncMismatch;
        private bool _ignoreRevisionError;

        public RevisionProcessor(Global g)
        {
            _g = g;
            _nodeProcessor = new NodeProcessor(g);
        }

        public bool CopyAuthor { get; set; }

        public bool CopyDateTime { get; set; }

        public bool CopySourceRevision { get; set; }

        public void ResyncRevision()
        {
            _g.Svn.Log(_g.Destination, _oneToHeadLog,
                     (s, e) =>
                     {
                         var p = e.RevisionProperties.FirstOrDefault(x => x.Key == PropertyKeySourceRevision);
                         if (p == null) return;
                         var sourceRivision = long.Parse(p.StringValue);
                         _g.RevisionMap.TrackRevision(sourceRivision, e.Revision);
                     });
            _resyncToRevision = _g.RevisionMap.GetLastRevision();
        }

        public void ProcessRevisionLog(SvnLogEventArgs e)
        {
            var sourceRevision = e.Revision;
            _g.Interaction.Info("{0} {1} {2} {3}", sourceRevision, e.Author, e.Time.ToLocalTime(), e.LogMessage);
            if (sourceRevision <= _resyncToRevision)
            {
                _firstLoad = false;
                if (!_ignoreResyncMismatch)
                    _g.Interaction.DoInteractively(
                        ref _ignoreResyncMismatch,
                        TitleErrorResyncRevision,
                        () => CheckResyncRevision(sourceRevision));
                return;
            }
            _g.Interaction.DoInteractively(
                ref _ignoreRevisionError,
                TitleProcessRevision,
                () =>
                {
                    var sourceTarget = new SvnUriTarget(_g.Source.Uri, e.Revision);
                    bool success = _firstLoad
                                      ? _nodeProcessor.ExportDirectory(sourceTarget, _g.WorkingDir)
                                      : ChangeWorkingCopy(e);

                    if (success)
                    {
                        CommitToDestination(e, true);
                        _firstLoad = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                });
        }

        private void CheckResyncRevision(long sourceRevision)
        {
            long destRevision = _g.RevisionMap.CheckResyncRevision(sourceRevision);
            if (destRevision > 0)
                _g.Interaction.Trace("\tResync {0} to {1}", sourceRevision, destRevision);
        }

        private void CommitToDestination(SvnLogEventArgs e, bool allowCopySourceRevision)
        {
            SvnCommitResult result;
            _g.Svn.Commit(_g.WorkingDir, new SvnCommitArgs { LogMessage = e.LogMessage }, out result);
            if (result == null) return;
            var sourceRevision = e.Revision;
            var destinationReivison = result.Revision;
            _g.RevisionMap.TrackRevision(sourceRevision, destinationReivison);
            if (CopyAuthor) _g.Svn.SetRevisionProperty(_g.Destination, destinationReivison, PropertyKeyAuthor, e.Author);
            if (CopyDateTime)
                _g.Svn.SetRevisionProperty(_g.Destination, destinationReivison, PropertyKeyDateTime, e.Time.ToString("O") + "Z");
            if (CopySourceRevision && allowCopySourceRevision)
                _g.Svn.SetRevisionProperty(_g.Destination, destinationReivison, PropertyKeySourceRevision,
                                         sourceRevision.ToString("#0"));
            _g.Svn.Update(_g.WorkingDir, _ignoreExternalUpdate);
            _g.Interaction.UpdateProgress(sourceRevision, destinationReivison);
        }

        /// <summary>
        /// Return false we didn't finish because _g._stopRequested, otherwise true.
        /// </summary>
        private bool ChangeWorkingCopy(SvnLogEventArgs e)
        {
            var changes = e.ChangedPaths;
            var itemsAdded = from x in changes
                             where x.Action == SvnChangeAction.Add
                             orderby x.Path
                             select x;

            var itemsModified = from x in changes
                                where x.Action == SvnChangeAction.Modify
                                select x;

            var itemsDeleted = from x in changes
                               where x.Action == SvnChangeAction.Delete
                               orderby x.Path descending
                               select x;

            if (!DetectNameChangeByCaseOnly(changes))
            {
                return ProcessNodes(itemsAdded, e, _nodeProcessor.Add) &&
                       ProcessNodes(itemsModified, e, _nodeProcessor.Modify) &&
                       ProcessNodes(itemsDeleted, e, _nodeProcessor.Delete);
            }

            var success = ProcessNodes(itemsDeleted, e, _nodeProcessor.Delete) &&
                          ProcessNodes(itemsModified, e, _nodeProcessor.Modify);
            if (!success) return false;

            CommitToDestination(e, false);

            return ProcessNodes(itemsAdded, e, _nodeProcessor.Add);
        }

        private bool DetectNameChangeByCaseOnly(IEnumerable<SvnChangeItem> changes)
        {
            var h = new HashSet<string>();
            foreach (var node in changes)
            {
                var p = node.Path.ToLowerInvariant();
                if (h.Contains(p)) return true;
                h.Add(p);
            }
            return false;
        }

        /// <summary>
        /// Return false if _g._stopRequested, otherwise true.
        /// </summary>
        private bool ProcessNodes(IEnumerable<SvnChangeItem> nodes, SvnLogEventArgs e, Action<SvnChangeItem, SvnLogEventArgs, string> action)
        {
            foreach (var node in nodes)
            {
                if (_g.StopRequested) return false; // canceled
                _nodeProcessor.ProcessNode(e, action, node);
            }
            return true; // finished 
        }
    }
}
