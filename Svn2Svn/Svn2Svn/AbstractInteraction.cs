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
namespace Svn2Svn
{
    /// <summary>
    /// Serve as base class for various logger implemenations.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractInteraction : IInteraction
    {
        public LogLevel Level { get; set; }

        public void Info(string value)
        {
            if (Level <= LogLevel.Info) Log(LogLevel.Info, value);
        }

        public void Info(string format, params object[] values)
        {
            if (Level <= LogLevel.Info) Log(LogLevel.Info, string.Format(format, values));
        }

        public void Trace(string value)
        {
            if (Level <= LogLevel.Trace) Log(LogLevel.Trace, value);
        }

        public void Trace(string format, params object[] values)
        {
            if (Level <= LogLevel.Trace) Log(LogLevel.Trace, string.Format(format, values));
        }

        public void Error(string value)
        {
            if (Level <= LogLevel.Error) Log(LogLevel.Error, value);
        }

        public void Error(string format, params object[] values)
        {
            if (Level <= LogLevel.Error) Log(LogLevel.Error, string.Format(format, values));
        }

        public virtual void UpdateProgress(long sourceRevision, long destinationReivison)
        {
            Info("#### {0} -> {1}", sourceRevision, destinationReivison);
        }

        protected abstract void Log(LogLevel level, string value);
    }
}