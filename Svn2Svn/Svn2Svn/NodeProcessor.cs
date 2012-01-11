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
using SharpSvn;

namespace Svn2Svn
{
    /// <summary>
    /// Logic to copy a node.
    /// </summary>
    /// <author>Kenneth Xu</author>
    internal class NodeProcessor
    {
        private const string TitleErrorProcessingNode = "Error Processing Node";
        private const string ActionModified = "\tModified ";
        private const string ActionCreated = "\tCreated ";
        private static readonly SvnAddArgs _forceAdd = new SvnAddArgs { Force = true };
        private static readonly SvnAddArgs _infiniteForceAdd = new SvnAddArgs{Depth = SvnDepth.Infinity, Force = true};
        private static readonly SvnExportArgs _infiniteOverwriteExport = new SvnExportArgs {Depth = SvnDepth.Infinity, Overwrite = true};

        private readonly Global _g;

        private bool _ignoreItemError;

        public NodeProcessor(Global g)
        {
            _g = g;
        }

        public bool ExportDirectory(SvnTarget sourceUri, string destinationPath)
        {
            using (var svnClient = new SvnClient())
            {
                svnClient.List(
                    sourceUri,
                    new SvnListArgs { Depth = SvnDepth.Infinity },
                    (s, e) => _g.Interaction.DoInteractively(
                        ref _ignoreItemError,
                        TitleErrorProcessingNode,
                        () => ExportDirectoryListItem(e, sourceUri.Revision, destinationPath)));
            }
            return !_g.StopRequested;
        }

        private void ExportDirectoryListItem(SvnListEventArgs e, SvnRevision revision, string destinationPath)
        {
            if (_g.StopRequested)
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
                _g.Svn.Export(source, destinationPath, _infiniteOverwriteExport);
            }
            if (destinationPath != _g.WorkingDir)
            {
                _g.Svn.Add(destinationPath, _forceAdd);
                _g.Interaction.Trace((exists ? ActionModified : ActionCreated) + destinationPath);
            }
            CopyProperties(source, destinationPath);
        }

        public void ProcessNode(SvnLogEventArgs e, Action<SvnChangeItem, SvnLogEventArgs, string> action, SvnChangeItem node)
        {
            var destinationPath = _g.Source.GetDestinationPath(_g.WorkingDir, node.Path);
            if (destinationPath == null) return;
            _g.Interaction.DoInteractively(
                ref _ignoreItemError,
                TitleErrorProcessingNode,
                () => action(node, e, destinationPath));
        }

        public void Delete(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
            {
                _g.Svn.Delete(destinationPath);
                _g.Interaction.Trace("\tDeleted " + destinationPath);
            }
        }

        public void Modify(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            var source = _g.Source.GetSourceTarget(node, e.Revision);
            if (node.NodeKind == SvnNodeKind.File)
            {
                _g.Svn.Export(source, destinationPath);
            }
            _g.Interaction.Trace(ActionModified + destinationPath);
            CopyProperties(source, destinationPath);
        }

        public void Add(SvnChangeItem node, SvnLogEventArgs e, string destinationPath)
        {
            if (destinationPath == _g.WorkingDir) return;
            var source = _g.Source.GetSourceTarget(node, e.Revision);
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
                _g.Svn.CreateDirectory(destinationPath);
                processed = true;
            }
            if (!processed)
            {
                _g.Svn.Export(source, destinationPath, _infiniteOverwriteExport);
                _g.Svn.Add(destinationPath, _infiniteForceAdd);
            }
            _g.Interaction.Trace(ActionCreated + destinationPath);
            CopyProperties(source, destinationPath);
        }

        private bool TryCopy(SvnChangeItem node, string destinationPath)
        {
            var relativePath = _g.Source.GetRelativePath(node.CopyFromPath);
            if (relativePath == null) return false;
            var revision = _g.RevisionMap.FindDestinationRevision(node.CopyFromRevision);
            if (revision < 0) return false;
            // must use server uri as working copy may have been delete when copy from old revision.
            var copyFromUri = new Uri(_g.Destination, relativePath);
            _g.Svn.Copy(new SvnUriTarget(copyFromUri, revision), destinationPath);
            return true;
        }

        private void CopyProperties(SvnTarget source, string destinationPath)
        {
            Collection<SvnPropertyListEventArgs> props;
            _g.Svn.GetPropertyList(source, out props);
            var keys = new HashSet<string>();
            foreach (var prop in props)
            {
                foreach (var p in prop.Properties)
                {
                    var key = p.Key;
                    keys.Add(key);
                    _g.Svn.SetProperty(destinationPath, key, p.RawValue);
                    _g.Interaction.Trace("\t\tSet {0}=>{1}", key, p.StringValue);
                }
            }
            _g.Svn.GetPropertyList(new SvnPathTarget(destinationPath), out props);
            foreach (var prop in props)
            {
                foreach (var p in prop.Properties)
                {
                    var key = p.Key;
                    if (keys.Contains(key)) continue;
                    _g.Svn.DeleteProperty(destinationPath, key);
                    _g.Interaction.Trace("\t\tDelete {0}", key);
                }
            }
        }
    }
}