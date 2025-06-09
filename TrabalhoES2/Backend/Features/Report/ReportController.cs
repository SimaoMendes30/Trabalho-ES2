using Backend.Domain.DTOs.Report;
using Backend.Features.Report.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Report;

[ApiController]
[Authorize]
[Route("api/reports")]                 // ⇦ muda aqui se quiseres continuar em PT
public sealed class ReportController : ControllerBase
{
    private readonly IReportService _svc;
    public ReportController(IReportService svc) => _svc = svc;

    /* ---------- NOVOS END-POINTS ----------------------------------- */

    [HttpGet("users/{id:int}/hours/day")]
    public async Task<ActionResult<IReadOnlyList<HoursPerDayDto>>>
        GetHoursPerDay(int id) =>
        Ok(await _svc.GetHoursPerDayAsync(id));

    [HttpGet("users/{id:int}/hours/month")]
    public async Task<ActionResult<IReadOnlyList<HoursPerMonthDto>>>
        GetHoursPerMonth(int id) =>
        Ok(await _svc.GetHoursPerMonthAsync(id));

    [HttpGet("users/{id:int}/cost/day")]
    public async Task<ActionResult<IReadOnlyList<CostPerDayDto>>>
        GetCostPerDay(int id) =>
        Ok(await _svc.GetCostPerDayAsync(id));

    [HttpGet("users/{id:int}/cost/month")]
    public async Task<ActionResult<IReadOnlyList<CostPerMonthDto>>>
        GetCostPerMonth(int id) =>
        Ok(await _svc.GetCostPerMonthAsync(id));

    /* ---------- END-POINTS  JÁ  EXISTENTES -------------------------- */

    [HttpGet("users/{id:int}/monthly")]
    public async Task<ActionResult<MonthlyUserReportDto>>
        GetUserMonthly(int id, [FromQuery] int year, [FromQuery] int month) =>
        Ok(await _svc.GetMonthlyUserReportAsync(id, year, month));

    [HttpGet("projects/monthly")]
    public async Task<ActionResult<IEnumerable<ProjectMonthlyReportDto>>>
        GetProjectsMonthly([FromQuery] int year, [FromQuery] int month) =>
        Ok(await _svc.GetMonthlyProjectReportsAsync(year, month));
}