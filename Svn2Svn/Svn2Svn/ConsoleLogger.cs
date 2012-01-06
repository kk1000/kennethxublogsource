using System;

namespace Svn2Svn
{
    public class ConsoleLogger : AbstractLogger
    {
        protected override void Log(LogLevel level, string value)
        {
            Console.WriteLine(value);
        }
    }
}