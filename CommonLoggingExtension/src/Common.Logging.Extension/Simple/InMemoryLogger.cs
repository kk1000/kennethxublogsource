#region License

/*
 * Copyright (C) 2009 the original author or authors.
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

namespace Common.Logging.Simple
{
    /// <summary>
    /// The logger that logs the entry in the memory and can be accessed
    /// via <see cref="LogEntries"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class InMemoryLogger : ILog
    {
        private readonly IList<InMemoryLogEntry> _logEntries = new List<InMemoryLogEntry>();
        private readonly string _name;
        private LogLevel _level = LogLevel.Off;

        public InMemoryLogger(string name, LogLevel level)
        {
            _name = name;
            _level = level;
        }

        public virtual IList<InMemoryLogEntry> LogEntries
        {
            get { return _logEntries; }
        }

        public virtual LogLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public virtual string Name
        {
            get { return _name; }
        }

        #region ILog Members

        public virtual void Debug(object message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public virtual void Debug(object message)
        {
            Debug(message, null);
        }

        public virtual void Error(object message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        public virtual void Error(object message)
        {
            Error(message, null);
        }

        public virtual void Fatal(object message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        public virtual void Fatal(object message)
        {
            Fatal(message, null);
        }

        public virtual void Info(object message, Exception exception)
        {
            Log(LogLevel.Info, message, exception);
        }

        public virtual void Info(object message)
        {
            Info(message, null);
        }

        public virtual bool IsDebugEnabled
        {
            get { return Level >= LogLevel.Debug; }
        }

        public virtual bool IsErrorEnabled
        {
            get { return Level >= LogLevel.Error; }
        }

        public virtual bool IsFatalEnabled
        {
            get { return Level >= LogLevel.Fatal; }
        }

        public virtual bool IsInfoEnabled
        {
            get { return Level >= LogLevel.Info; }
        }

        public virtual bool IsTraceEnabled
        {
            get { return Level >= LogLevel.Trace; }
        }

        public virtual bool IsWarnEnabled
        {
            get { return Level >= LogLevel.Warn; }
        }

        public virtual void Trace(object message, Exception exception)
        {
            Log(LogLevel.Trace, message, exception);
        }

        public virtual void Trace(object message)
        {
            Trace(message, null);
        }

        public virtual void Warn(object message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception);
        }

        public virtual void Warn(object message)
        {
            Warn(message, null);
        }

        #endregion

        private void Log(LogLevel level, object message, Exception exception)
        {
            if (level < _level) return;

            InMemoryLogEntry e = new InMemoryLogEntry();
            e.Message = message;
            e.LogLevel = level;
            e.Exception = exception;
            e.EventTime = DateTime.Now;
            _logEntries.Add(e);
        }
    }
}
