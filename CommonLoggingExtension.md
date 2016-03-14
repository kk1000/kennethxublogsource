Extensions to [Common.Logging](http://netcommon.sourceforge.net/)

# Introduction #

Binary is available in the downloads area and source code can be checked out from http://kennethxublogsource.googlecode.com/svn/trunk/CommonLoggingExtension/

# Features #

  * [An in memory sink that simplifies unit testing code that uses Common.Logging.](http://kennethxu.blogspot.com/2009/04/commonlogging-for-net-and-it-use-in.html)
    * Each InMemoryLogger maintains its own sink, so you can write assertions easily.
    * InMemoryLogger's log level is by default dynamically connected to the log level of the InMemoryLoggerFactoryAdapter. This let you change log level of ALL InMemoryLogger by just one call to the factory adapter. So that you can quickly write unit test case with parametrized test feature to ensure the code works with logging enabled and disabled.
    * Level on each logger can be certainly overridden.
    * The InMemoryLoggerFactoryAdapter provides method to obtain the InMemoryLogger instance by name or type. The return type is InMemoryLogger by nature so you don't have to cast it from ILog.
    * The factory adapter has ClearAll method to clear all sinks maintained by the loggers.
    * factory adapter has ResetAllLevel method to reset the level of all loggers to make them back in sync with factory log level.

## Examples ##
  * In memory sick used in [OdpNetDataReaderWrapperTest](http://code.google.com/p/kennethxublogsource/source/browse/trunk/SpringExtension/test/Spring.Data.Extension.Tests/Data/Support/OdpNetDataReaderWrapperTest.cs#129)