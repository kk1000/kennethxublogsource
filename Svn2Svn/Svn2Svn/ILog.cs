namespace Svn2Svn
{
    public interface ILog
    {
        void Info(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Info(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void Trace(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Trace(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void Error(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Error(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void UpdateProgress(long sourceRevision, long destinationReivison);
    }
}