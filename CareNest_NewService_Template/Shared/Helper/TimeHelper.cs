namespace Shared.Helper
{
    public static class TimeHelper
    {
        public static DateTime ToUtc(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime ToLocal(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        public static string ToIsoString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }
}
