namespace Backend.Domain.DTOs.Report;
public record ProjectUserHoursDto(
    int    UserId,
    string UserName,
    double Hours,
    decimal Cost);