using Backend.Domain.DTOs.Report;

namespace Backend.Features.Report.Interfaces;

public interface IReportRepository
{
    Task<IReadOnlyList<HoursPerDayDto>>   GetHoursPerDayAsync  (int userId);
    Task<IReadOnlyList<HoursPerMonthDto>> GetHoursPerMonthAsync(int userId);
    Task<IReadOnlyList<CostPerDayDto>>    GetCostPerDayAsync   (int userId);
    Task<IReadOnlyList<CostPerMonthDto>>  GetCostPerMonthAsync (int userId);
}