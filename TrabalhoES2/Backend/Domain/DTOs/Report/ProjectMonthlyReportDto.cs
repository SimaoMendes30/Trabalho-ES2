namespace Backend.Domain.DTOs.Report;
public record ProjectMonthlyReportDto(
    int                               ProjectId,
    string                            ProjectName,
    string?                           ClientName,
    int                               Year,
    int                               Month,
    IReadOnlyList<DailyTaskDto>       Tasks,
    IReadOnlyList<ProjectUserHoursDto> Users,
    double                            TotalHours,
    decimal                           TotalCost);