using System;
using System.Diagnostics;

namespace Appmilla.RestApiClient.Extensions
{
    public static class IntExtensions
    {
        [DebuggerStepThrough]
        public static TimeSpan Ticks(this int number)
        {
            return TimeSpan.FromTicks(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this int number)
        {
            return TimeSpan.FromMilliseconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Seconds(this int number)
        {
            return TimeSpan.FromSeconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Minutes(this int number)
        {
            return TimeSpan.FromMinutes(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Hours(this int number)
        {
            return TimeSpan.FromHours(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Days(this int number)
        {
            return TimeSpan.FromDays(number);
        }
    }
}
