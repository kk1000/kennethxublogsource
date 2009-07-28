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

#if !CL_2_0
using System;

namespace Common.Logging.Factory
{
    /// <summary>
    /// Provides common method implementation for <see cref="ILog"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractLogger : ILog
    {
        #region ILog Members

        public virtual void Debug(object message, Exception exception)
        {
            WriteInternal(LogLevel.Debug, message, exception);
        }

        public virtual void Debug(object message)
        {
            Debug(message, null);
        }

        public virtual void Error(object message, Exception exception)
        {
            WriteInternal(LogLevel.Error, message, exception);
        }

        public virtual void Error(object message)
        {
            Error(message, null);
        }

        public virtual void Fatal(object message, Exception exception)
        {
            WriteInternal(LogLevel.Fatal, message, exception);
        }

        public virtual void Fatal(object message)
        {
            Fatal(message, null);
        }

        public virtual void Info(object message, Exception exception)
        {
            WriteInternal(LogLevel.Info, message, exception);
        }

        public virtual void Info(object message)
        {
            Info(message, null);
        }

        public virtual void Trace(object message, Exception exception)
        {
            WriteInternal(LogLevel.Trace, message, exception);
        }

        public virtual void Trace(object message)
        {
            Trace(message, null);
        }

        public virtual void Warn(object message, Exception exception)
        {
            WriteInternal(LogLevel.Warn, message, exception);
        }

        public virtual void Warn(object message)
        {
            Warn(message, null);
        }

        public abstract bool IsDebugEnabled { get; }

        public abstract bool IsErrorEnabled { get; }

        public abstract bool IsFatalEnabled { get; }

        public abstract bool IsInfoEnabled { get; }

        public abstract bool IsTraceEnabled { get; }

        public abstract bool IsWarnEnabled { get; }

        #endregion

        protected abstract void WriteInternal(LogLevel level, object message, Exception exception);
    }
}
#endif
