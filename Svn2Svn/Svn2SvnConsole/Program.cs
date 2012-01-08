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
using Svn2Svn;

namespace Svn2SvnConsole
{
    class Program
    {
        private readonly string _destinationUri;
        private readonly string _sourceUri;
        private readonly string _workingDir;
        private readonly LogLevel _logLevel = LogLevel.Error;
        private readonly bool _ignoreError;

        private bool _copyAuthor = true;
        private bool _copyDateTime = true;
        private bool _copySourceRevision = true;
        private long _startRevision;
        private long _endRevision = -1;

        static void Main(string[] args)
        {
            var p = new Program(args);
            p.Run();
        }

        public Program(string[] args)
        {
            var remaining = new List<string>();
            foreach (var s in args)
            {
                if (s[0] == '-')
                {
                    var o = s.ToLower();
                    if (o == "-v")
                    {
                        if (_logLevel > LogLevel.Info) _logLevel = LogLevel.Info;
                    }
                    else if (o == "-v+") _logLevel = LogLevel.Trace;
                    else if (o == "-i") _ignoreError = true;
                    else if (o == "-x") DoNotCopyRevisionProperties();
                    else if (o.StartsWith("-r:")) ParseRevision(s);
                    else if (o.StartsWith("-x:")) ParseCopyRevisionProperty(s);
                    else Usage(s);
                }
                else
                {
                    remaining.Add(s);
                }
            }
            if (remaining.Count != 3) Usage(null);
            _sourceUri = remaining[0];
            _destinationUri = remaining[1];
            _workingDir = remaining[2];
            //Console.WriteLine("log level: {0}", _logLevel);
            //Console.WriteLine("ignore error: {0}", _ignoreError);
            //Console.WriteLine("copy author: {0}", _copyAuthor);
            //Console.WriteLine("copy date/time: {0}", _copyDateTime);
            //Console.WriteLine("copy source revision: {0}", _copySourceRevision);
            //Console.WriteLine("revision: {0} : {1}", _startRevision, _endRevision);
        }

        void Run()
        {
            var copier = new Copier(new Uri(_sourceUri), new Uri(_destinationUri),_workingDir)
            {
                Interaction = _ignoreError ? new IgnoreErrorInteraction() : new ConsoleInteraction(),
                CopyAuthor = _copyAuthor,
                CopyDateTime = _copyDateTime,
                CopySourceRevision = _copySourceRevision,
                StartRevision = _startRevision,
                EndRevision = _endRevision,
            };
            copier.Copy();

        }

        private void DoNotCopyRevisionProperties()
        {
            _copyAuthor = false;
            _copyDateTime = false;
            _copySourceRevision = false;
        }

        private void ParseCopyRevisionProperty(string option)
        {
            if (option.Length == 3) Usage(option);
            foreach (var c in option)
            {
                switch (c)
                {
                    case '-':
                    case ':':
                    case 'x':
                    case 'X':
                        continue;
                    case 'A':
                    case 'a':
                        _copyAuthor = false;
                        continue;
                    case 'D':
                    case 'd':
                        _copyDateTime = false;
                        continue;
                    case 'R':
                    case 'r':
                        _copySourceRevision = false;
                        continue;
                    default:
                        Usage(option);
                        break;
                }
            }
        }

        private void ParseRevision(string s)
        {
            int position = s.IndexOf(':', 3);
            var from = s.Substring(3, (position < 0 ? s.Length : position) - 3);
            if (from.Length > 0 && !long.TryParse(from, out _startRevision))
            {
                Usage(s);
            }
            if (position >= 0 && !long.TryParse(s.Substring(position + 1), out _endRevision))
            {
                Usage(s);
            }
        }

        private void Usage(string s)
        {
            if (s != null) Console.Error.WriteLine("Invalid option: " + s);
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("\tSvn2SvnConsole.exe [options] sourceUri destinationUri workingDir");
            Console.Error.WriteLine("options:");
            Console.Error.WriteLine("\t-R:from:to Copy revisions specified by from and to.");
            Console.Error.WriteLine("\t-R:start   Copy revisions from specified start to HEAD.");
            Console.Error.WriteLine("\t-R::end    Copy revisions from 0 to specified end.");
            Console.Error.WriteLine("\t-X         Do not copy any revision property.");
            Console.Error.WriteLine("\t-X:[ADR]   Do not copy one or more revision properties. e.g.:");
            Console.Error.WriteLine("\t           -X:D  - Do not copy date/time revision property.");
            Console.Error.WriteLine("\t           -X:AD - Do not copy author and date/time revision property.");
            Console.Error.WriteLine("\t-I         Ignore all none fatal errors. Only log them.");
            Console.Error.WriteLine("\t-V         Log every revision.");
            Console.Error.WriteLine("\t-V+        Log every revision and node.");
            Environment.Exit(1);
        }
    }
}
