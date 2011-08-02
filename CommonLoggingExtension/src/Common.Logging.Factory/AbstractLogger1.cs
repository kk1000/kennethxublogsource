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


#if NLOG
namespace Common.Logging.NLog2
{
    public partial class NLogLogger : ILog
#endif
#if !ADAPTER
namespace Common.Logging.Factory
{    /// <summary>
    /// Provides common method implementation for <see cref="ILog"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public abstract class AbstractLogger : ILog
#endif
    {

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Debug"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Debug(object message, Exception exception)
        {
            WriteInternal(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Debug(object message)
        {
            Debug(message, null);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Error"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Error(object message, Exception exception)
        {
            WriteInternal(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Error"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Error(object message)
        {
            Error(message, null);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Fatal"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Fatal(object message, Exception exception)
        {
            WriteInternal(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Fatal"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Fatal(object message)
        {
            Fatal(message, null);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Info"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Info(object message, Exception exception)
        {
            WriteInternal(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Info"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Info(object message)
        {
            Info(message, null);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Trace"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Trace(object message, Exception exception)
        {
            WriteInternal(LogLevel.Trace, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Trace(object message)
        {
            Trace(message, null);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Warn"/> level including
        ///             the stack trace of the <see cref="T:System.Exception"/> passed
        ///             as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param><param name="exception">The exception to log, including its stack trace.</param>
        public virtual void Warn(object message, Exception exception)
        {
            WriteInternal(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Warn"/> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public virtual void Warn(object message)
        {
            Warn(message, null);
        }

#if !ADAPTER
        public abstract bool IsDebugEnabled { get; }

        public abstract bool IsErrorEnabled { get; }

        public abstract bool IsFatalEnabled { get; }

        public abstract bool IsInfoEnabled { get; }

        public abstract bool IsTraceEnabled { get; }

        public abstract bool IsWarnEnabled { get; }

        protected abstract void WriteInternal(LogLevel level, object message, Exception exception);
#endif
    }
}
