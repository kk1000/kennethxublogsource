#region License

/*
 * Copyright 2002-2009 the original author or authors.
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
using System.Configuration;
using NLog;
using NLog.Targets;

namespace Common.Logging.NLog2
{
    /// <summary>
    /// Routes all log events logged through NLog into the Common.Logging infrastructure.
    /// </summary>
    /// <remarks>
    /// <example>
    /// To route all NLog events to Common.Logging, you must add this target to your configuration:
    /// <code>
    /// LoggingConfiguration cfg = new LoggingConfiguration();
    /// CommonLoggingTarget target = new CommonLoggingTarget(&quot;${level:uppercase=true}|${logger}|${message}&quot;);
    /// cfg.LoggingRules.Add(new LoggingRule(&quot;*&quot;, LogLevel.Trace, target));
    /// 
    /// LogManager.Configuration = cfg;
    /// 
    /// Logger log = LogManager.GetLogger(&quot;mylogger&quot;);
    /// log.Debug(&quot;some message&quot;);
    /// </code>
    /// </example>
    /// </remarks>
    /// <author>Erich Eichinger</author>
    [Target("CommonLogging")]
    public class CommonLoggingTarget : TargetWithLayout
    {
        private delegate string MessageGetter();
        private delegate void LogMethod(ILog logger, MessageGetter fmtr, Exception exception);

        private static readonly Dictionary<NLog.LogLevel, LogMethod> logMethods =
                new Dictionary<NLog.LogLevel, LogMethod>
                    {
                        {NLog.LogLevel.Trace, Trace},
                        {NLog.LogLevel.Debug, Debug},
                        {NLog.LogLevel.Info, Info},
                        {NLog.LogLevel.Warn, Warn},
                        {NLog.LogLevel.Error, Error},
                        {NLog.LogLevel.Fatal, Fatal},
                        {NLog.LogLevel.Off, (log, msg, ex) => { }},
                    };

        /// <summary>
        /// Writes the event to the Common.Logging infrastructure
        /// </summary>
        protected override void Write(LogEventInfo logEvent)
        {
            if (LogManager.Adapter is NLogLoggerFactoryAdapter)
            {
                throw new ConfigurationErrorsException("routing NLog events to Common.Logging configured with NLogLoggerFactoryAdapter results in an endless recursion");
            }

            ILog logger = LogManager.GetLogger(logEvent.LoggerName);
            LogMethod log = logMethods[logEvent.Level];
            log(logger, () => Layout.Render(logEvent), logEvent.Exception);
        }

        private static void Trace(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsTraceEnabled) log.Trace(getMsg(), ex);
        }

        private static void Debug(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsDebugEnabled) log.Debug(getMsg(), ex);
        }

        private static void Info(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsInfoEnabled) log.Info(getMsg(), ex);
        }

        private static void Warn(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsWarnEnabled) log.Warn(getMsg(), ex);
        }

        private static void Error(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsErrorEnabled) log.Error(getMsg(), ex);
        }

        private static void Fatal(ILog log, MessageGetter getMsg, Exception ex)
        {
            if (log.IsFatalEnabled) log.Fatal(getMsg(), ex);
        }
    }
}