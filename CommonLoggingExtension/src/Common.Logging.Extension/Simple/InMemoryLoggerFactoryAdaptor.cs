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

        public virtual InMemoryLogger GetInMemoryLogger(string name)
        {
            lock (_repository)
            {
                InMemoryLogger logger;
                if (!_repository.TryGetValue(name, out logger))
                {
                    logger = new InMemoryLogger(name, Level);
                    _repository[name] = logger;
                }
                return logger;
            }
        }

        public virtual InMemoryLogger GetInMemoryLogger(Type type)
        {
            return GetInMemoryLogger(type.FullName);
        }

        public virtual void ClearAll()
        {
            lock (_repository)
            {
                foreach (var logger in _repository.Values)
                {
                    logger.LogEntries.Clear();
                }
            }
        }
    }
}
