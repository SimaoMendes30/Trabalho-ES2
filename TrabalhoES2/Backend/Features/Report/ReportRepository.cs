using Backend.Domain.DTOs.Report;
using Backend.Features.Report.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Report;

/// <summary>
/// Agregados de horas e custos para RF24 – RF26.
/// </summary>
public sealed class ReportRepository : IReportRepository
{
    private readonly IDbContextFactory<SgscDbContext> _factory;

    public ReportRepository(IDbContextFactory<SgscDbContext> factory) =>
        _factory = factory;

    /* ───────────── HORAS ───────────── */

    public async Task<IReadOnlyList<HoursPerDayDto>> GetHoursPerDayAsync(int userId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        return await ctx.Tarefa
            .Where(t => t.Responsavel == userId &&
                        !t.IsDeleted &&
                        t.DataFim != null &&
                        t.Estado == "Concluído")
            .AsNoTracking()
            // 1️⃣ Agrupamos por DateTime.Date (EF traduz)
            .GroupBy(t => t.DataFim!.Value.Date)
            // 2️⃣ Convertes para DateOnly só na projecção
            .Select(g => new HoursPerDayDto(
                DateOnly.FromDateTime(g.Key),
                Math.Round(g.Sum(t => (t.DataFim!.Value - t.DataInicio).TotalHours), 2)))
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<HoursPerMonthDto>> GetHoursPerMonthAsync(int userId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        // 1) Agrega na BD (sem Round)
        var bruto = await ctx.Tarefa
            .Where(t => t.Responsavel == userId &&
                        !t.IsDeleted &&
                        t.DataFim != null &&
                        t.Estado == "Concluído")
            .AsNoTracking()
            .GroupBy(t => new { t.DataFim!.Value.Year, t.DataFim.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Sum(t => (t.DataFim!.Value - t.DataInicio).TotalHours)
            })
            .ToListAsync();                // ← agora já estamos em memória

        // 2) Faz o Round em LINQ-to-Objects
        var resultado = bruto
            .Select(x => new HoursPerMonthDto(
                x.Month,
                x.Year,
                Math.Round(x.Total, 2)))
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();

        return resultado;
    }


    /* ───────────── CUSTO ───────────── */

    public async Task<IReadOnlyList<CostPerDayDto>> GetCostPerDayAsync(int userId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        // Trazemos para memória para poder usar lógica imperativa.
        var tasks = await ctx.Tarefa
            .Include(t => t.IdProjetos)
            .Where(t => t.Responsavel == userId &&
                        !t.IsDeleted &&
                        t.DataFim != null &&
                        t.Estado == "Concluído")
            .AsNoTracking()
            .ToListAsync();

        return tasks
            .GroupBy(t => DateOnly.FromDateTime(t.DataFim!.Value.Date))
            .Select(g =>
            {
                var totalCost = g.Sum(t =>
                {
                    var hours = (t.DataFim!.Value - t.DataInicio).TotalHours;
                    var rate  = t.PrecoHora
                             ?? t.IdProjetos.FirstOrDefault()?.PrecoHora
                             ?? 0M;
                    return (decimal)hours * rate;
                });
                return new CostPerDayDto(g.Key, Math.Round(totalCost, 2));
            })
            .OrderBy(r => r.Date)
            .ToList();
    }

    public async Task<IReadOnlyList<CostPerMonthDto>> GetCostPerMonthAsync(int userId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        var tasks = await ctx.Tarefa
            .Include(t => t.IdProjetos)
            .Where(t => t.Responsavel == userId &&
                        !t.IsDeleted &&
                        t.DataFim != null &&
                        t.Estado == "Concluído")
            .AsNoTracking()
            .ToListAsync();

        return tasks
            .GroupBy(t => new { t.DataFim!.Value.Year, t.DataFim.Value.Month })
            .Select(g =>
            {
                var totalCost = g.Sum(t =>
                {
                    var hours = (t.DataFim!.Value - t.DataInicio).TotalHours;
                    var rate  = t.PrecoHora
                             ?? t.IdProjetos.FirstOrDefault()?.PrecoHora
                             ?? 0M;
                    return (decimal)hours * rate;
                });
                return new CostPerMonthDto(g.Key.Month, g.Key.Year, Math.Round(totalCost, 2));
            })
            .OrderBy(r => r.Year).ThenBy(r => r.Month)
            .ToList();
    }
}
