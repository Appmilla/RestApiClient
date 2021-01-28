using System;

namespace Appmilla.RestApiClient.Logging
{
    [Flags]
    public enum LoggingLevel
    {
        Verbose = 0,
        Debug = 1,
        Error = 2,
        Fatal = 4,
        Information = 8,
        Warning = 16,
        All = Debug | Error | Fatal | Information | Warning
    }
    
    public enum LogEventType
    {
        ApiCall        
    }    
}
