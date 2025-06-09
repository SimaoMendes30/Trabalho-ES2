namespace Backend.Domain.DTOs.Report;
public record DailyReportEntryDto(
    DateOnly                    Day,
    IReadOnlyList<DailyTaskDto> Tasks,
    double                      TotalHours,
    decimal                     TotalCost,
    bool                        OverHours);