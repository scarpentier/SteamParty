using System;

namespace SteamParty.Core
{
    public static class DateTimeExtension
    {
        public static DateTime FromUnix(long timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}