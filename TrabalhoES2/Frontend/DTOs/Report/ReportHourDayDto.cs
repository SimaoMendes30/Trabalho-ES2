namespace Frontend.DTOs.Report;

public class ReportHourDayDto
{
    public DateOnly Date { get; set; }
    public double TotalHours { get; set; }

    // Campo calculado no frontend
    public bool ExceedsLimit => TotalHours > 8;
}