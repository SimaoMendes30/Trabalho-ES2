namespace Frontend.DTOs.Report;

public class MonthlyUserReportDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<DailyReportEntryDto> Entries { get; set; } = new();
    public double TotalHours { get; set; }
    public decimal TotalCost { get; set; }
}

public class DailyReportEntryDto
{
    public DateOnly Day { get; set; }
    public List<DailyTaskDto> Tasks { get; set; } = new();
    public double TotalHours { get; set; }
    public decimal TotalCost { get; set; }
    public bool OverHours { get; set; }
}

public class DailyTaskDto
{
    public int TaskId { get; set; }
    public string Title { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string? ClientName { get; set; }
    public double Hours { get; set; }
    public decimal Cost { get; set; }
}