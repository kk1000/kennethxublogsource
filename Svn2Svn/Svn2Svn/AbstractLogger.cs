namespace Svn2Svn
{
    public abstract class AbstractLogger : ILog
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
            
        }

        protected abstract void Log(LogLevel level, string value);
    }
}