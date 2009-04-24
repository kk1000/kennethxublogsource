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
    /// Logger factory that keeps the log information in the memory mainly
    /// for unit test.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class InMemoryLoggerFactoryAdaptor : ILoggerFactoryAdapter
    {
        readonly IDictionary<string, InMemoryLogger> _repository = 
            new Dictionary<string, InMemoryLogger>();

        private LogLevel _level = LogLevel.Off;

        /// <summary>
        /// Gets and sets the level used by loggers created by this
        /// factory and have not overriden their log levels.
        /// </summary>
        /// <seealso cref="InMemoryLogger.Level"/>
        public virtual LogLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        #region ILoggerFactoryAdapter Members

        public virtual ILog GetLogger(string name)
        {
            return GetInMemoryLogger(name);
        }

        public virtual ILog GetLogger(Type type)
        {
            return GetInMemoryLogger(type);
        }

        #endregion

        /// <summary>
        /// Get the logger with given <paramref name="name"/>. Create new if one
        /// does not already exist.
        /// </summary>
        /// <param name="name">The name of logger.</param>
        /// <returns>An instance of <see cref="InMemoryLogger"/></returns>
        public virtual InMemoryLogger GetInMemoryLogger(string name)
        {
            lock (_repository)
            {
                InMemoryLogger logger;
                if (!_repository.TryGetValue(name, out logger))
                {
                    logger = new InMemoryLogger(name, this);
                    _repository[name] = logger;
                }
                return logger;
            }
        }

        /// <summary>
        /// Get the logger for given <paramref name="type"/>. Create new if one
        /// does not already exist.
        /// </summary>
        /// <param name="type">The type of logging class.</param>
        /// <returns>An instance of <see cref="InMemoryLogger"/></returns>
        public virtual InMemoryLogger GetInMemoryLogger(Type type)
        {
            return GetInMemoryLogger(type.FullName);
        }

        /// <summary>
        /// Clears all <see cref="InMemoryLogEntry"/> in all the loggers.
        /// </summary>
        public virtual void ClearAll()
        {
            ActOnAllLoggers(logger => logger.LogEntries.Clear());
        }

        /// <summary>
        /// Reset the log level of all existing loggers.
        /// </summary>
        public virtual void ResetAllLevel()
        {
            ActOnAllLoggers(logger => logger.ResetLevel());
        }

        /// <summary>
        /// Execute <paramref name="action"/> on all loggers created by this 
        /// factory.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public virtual void ActOnAllLoggers(Action<InMemoryLogger> action)
        {
            lock (_repository)
            {
                foreach (var logger in _repository.Values)
                {
                    action(logger);
                }
            }
        }
    }
}
