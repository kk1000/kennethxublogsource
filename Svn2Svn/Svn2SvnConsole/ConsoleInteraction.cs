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
using Svn2Svn;

namespace Svn2SvnConsole
{
    /// <summary>
    /// Send log to console.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class ConsoleInteraction : AbstractInteraction
    {
        protected override void Log(LogLevel level, string value)
        {
            Console.WriteLine(value);
        }

        public override ErrorDisposition Ask(string title, string message)
        {
            Console.WriteLine(title);
            Console.WriteLine(message);
            Console.Write("Please select your option Fail/Retry/Ignore/ignore All:");
            while (true)
            {
                var c = Console.ReadKey();
                ErrorDisposition result; 
                switch (c.KeyChar)
                {
                    case 'f':
                    case 'F':
                        result = ErrorDisposition.Fail;
                        break;
                    case 'r':
                    case 'R':
                        result = ErrorDisposition.Retry;
                        break;
                    case 'i':
                    case 'I':
                        result = ErrorDisposition.Ignore;
                        break;
                    case 'a':
                    case 'A':
                        result = ErrorDisposition.IgnoreAll;
                        break;
                    default:
                        continue;
                }
                Console.WriteLine();
                return result;
            }
        }
    }
}