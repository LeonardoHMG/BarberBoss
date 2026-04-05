namespace BarberBoss.Application.Utilities;
public static class DateHelper
{
    public static (DateOnly startDate, DateOnly endDate) GetWeek(DateOnly? date)
    {
        var referenceDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var daysFromMonday = ((int)referenceDate.DayOfWeek - (int)DayOfWeek.Monday +7) % 7;

        var startDate = referenceDate.AddDays(-daysFromMonday);
        var endDate = startDate.AddDays(6);

        return (startDate, endDate);
    }
}
