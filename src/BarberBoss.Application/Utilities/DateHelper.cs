namespace BarberBoss.Application.Utilities;
public static class DateHelper
{
    public static (DateTime StartDate, DateTime EndDate) GetWeek(DateOnly? date)
    {
        var referenceDate = date ?? DateOnly.FromDateTime(DateTime.Now);

        var daysFromMonday = ((int)referenceDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var monday = referenceDate.AddDays(-daysFromMonday);
        var sunday = monday.AddDays(6);

        var startDateTime = monday.ToDateTime(TimeOnly.MinValue);
        var endDateTime = sunday.ToDateTime(TimeOnly.MaxValue);   

        return (startDateTime, endDateTime);
    }
}
