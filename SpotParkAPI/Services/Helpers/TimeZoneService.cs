using System;

namespace SpotParkAPI.Services.Helpers
{
    public static class TimeZoneService
    {
        private static readonly TimeZoneInfo RomaniaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

        public static TimeSpan ConvertLocalToUtc(TimeSpan localTime)
        {
            var today = DateTime.UtcNow.Date;
            var localDateTime = today.Add(localTime);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, RomaniaTimeZone);
            return utcDateTime.TimeOfDay;
        }

        public static DateTime ConvertLocalToUtc(DateTime localDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, RomaniaTimeZone);
        }

        public static DateTime ConvertUtcToLocal(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, RomaniaTimeZone);
        }
    }
}
