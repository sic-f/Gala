namespace Utility.Helpers;

public class DateHelper
{
    public static string GetFormattedDateTimeOffset(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.sssssssZ");
    }
}
