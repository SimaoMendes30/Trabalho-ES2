namespace Backend.Domain.DTOs.Report;
public record DailyTaskDto(
    int    TaskId,
    string Title,
    int    ProjectId,
    string ProjectName,
    string? ClientName,
    double Hours,
    decimal Cost);