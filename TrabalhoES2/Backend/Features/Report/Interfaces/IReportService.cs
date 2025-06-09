// Features/Report/Interfaces/IReportService.cs
using Backend.Domain.DTOs.Report;

namespace Backend.Features.Report.Interfaces;

public interface IReportService
{
    // existentes
    Task<MonthlyUserReportDto>                GetMonthlyUserReportAsync   (int userId, int year, int month);
    Task<IEnumerable<ProjectMonthlyReportDto>> GetMonthlyProjectReportsAsync(int year, int month);

    // novos
    Task<IReadOnlyList<HoursPerDayDto>>   GetHoursPerDayAsync  (int userId);
    Task<IReadOnlyList<HoursPerMonthDto>> GetHoursPerMonthAsync(int userId);
    Task<IReadOnlyList<CostPerDayDto>>    GetCostPerDayAsync   (int userId);
    Task<IReadOnlyList<CostPerMonthDto>>  GetCostPerMonthAsync (int userId);
}
