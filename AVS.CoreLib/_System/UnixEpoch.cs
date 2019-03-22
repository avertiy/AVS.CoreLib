using System;

namespace AVS.CoreLib._System
{
    public static class UnixEpoch
    {
        public static readonly DateTime DateTimeUnixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime UnixTimeStampToDateTime(this ulong unixTimeStamp)
        {
            return DateTimeUnixEpochStart.AddSeconds(unixTimeStamp);
        }

        public static ulong DateTimeToUnixTimeStamp(this DateTime dateTime)
        {
            return (ulong)Math.Floor(dateTime.Subtract(DateTimeUnixEpochStart).TotalSeconds);
        }
    }
}