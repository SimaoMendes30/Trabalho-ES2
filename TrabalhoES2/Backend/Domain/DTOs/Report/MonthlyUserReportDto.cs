namespace Backend.Domain.DTOs.Report;
public record MonthlyUserReportDto(
    int                               UserId,
    string                            UserName,
    int                               Year,
    int                               Month,
    IReadOnlyList<DailyReportEntryDto> Entries,
    double                            TotalHours,
    decimal                           TotalCost);