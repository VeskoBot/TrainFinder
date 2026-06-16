namespace TrainFinder.Application.Helpers;

public static class TimeHelper
{
    private static readonly TimeZoneInfo EasternEuropeanTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

    public static DateTime GetEasternEuropeanTime()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EasternEuropeanTimeZone);
    }

    public static DateTime ToEasternEuropeanTime(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EasternEuropeanTimeZone);
    }
}
