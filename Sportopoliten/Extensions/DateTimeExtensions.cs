namespace Sportopoliten.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToKyivTime(this DateTime utcDateTime)
        {
            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, kyivTimeZone);
        }
    }
}