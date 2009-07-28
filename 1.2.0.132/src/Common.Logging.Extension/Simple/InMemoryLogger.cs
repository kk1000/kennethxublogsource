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
using Common.Logging.Factory;

namespace Common.Logging.Simple
{
    /// <summary>
    /// The logger that logs the entry in the memory and can be accessed
    /// via <see cref="LogEntries"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    /// <author>Jan Willem Boer, janwillem@jwenr.nl</author>
    public class InMemoryLogger : AbstractLogger
    {
        private readonly IList<InMemoryLogEntry> _logEntries = new List<InMemoryLogEntry>();
        private readonly string _name;
        private LogLevel? _level;
        private readonly InMemoryLoggerFactoryAdapter _factory;

        /// <summary>
        /// Construct a new instance of <see cref="InMemoryLogger"/>.
        /// </summary>
        /// <param name="name">The name of the logger to construct.</param>
        /// <param name="factory">The factory this logger belongs to.</param>
        public InMemoryLogger(string name, InMemoryLoggerFactoryAdapter factory)
        {
            _name = name;
            _factory = factory;
        }

        /// <summary>
        /// Gets the list of <see cref="InMemoryLogEntry"/> logged by this 
        /// logger in memory.
        /// </summary>
        public virtual IList<InMemoryLogEntry> LogEntries
        {
            get { return _logEntries; }
        }

        /// <summary>
        /// Gets and sets the log level of this logger.
        /// </summary>
        /// <remarks>
        /// If the log lever of current logger was never set or got reset by 
        /// <see cref="ResetLevel"/> method, then the log level is the 
        /// current value of <see cref="InMemoryLoggerFactoryAdapter.Level"/>
        /// of the <see cref="InMemoryLoggerFactoryAdapter"/> that created
        /// this logger.
        /// </remarks>
        /// <seealso cref="ResetLevel"/>
        public virtual LogLevel Level
        {
            get { return _level??_factory.Level; }
            set { _level = value; }
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// This clears previous set log <see cref="Level"/> of this logger.
        /// </summary>
        /// <seealso cref="Level"/>
        public void ResetLevel()
        {
            _level = null;
        }

        #region abstractlogger overrides
        public override bool IsDebugEnabled
        {
            get { return Level >= LogLevel.Debug; }
        }

        public override bool IsErrorEnabled
        {
            get { return Level >= LogLevel.Error; }
        }

        public override bool IsFatalEnabled
        {
            get { return Level >= LogLevel.Fatal; }
        }

        public override bool IsInfoEnabled
        {
            get { return Level >= LogLevel.Info; }
        }

        public override bool IsTraceEnabled
        {
            get { return Level >= LogLevel.Trace; }
        }

        public override bool IsWarnEnabled
        {
            get { return Level >= LogLevel.Warn; }
        }
        #endregion

        /// <summary>
        /// log writer function that is called by abstractlogger
        /// </summary>
        protected override void WriteInternal(LogLevel level, object message, Exception exception)
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
