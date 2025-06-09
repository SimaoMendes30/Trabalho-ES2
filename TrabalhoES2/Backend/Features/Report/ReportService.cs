using Backend.Domain.DTOs.Report;
using Backend.Features.Report.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Report;

/// <summary>
/// Serviço de relatórios (RF23-RF28).
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly IReportRepository                _repo;
    private readonly IDbContextFactory<SgscDbContext> _factory;

    public ReportService(IReportRepository                repo,
                         IDbContextFactory<SgscDbContext> factory)
    {
        _repo    = repo;
        _factory = factory;
    }

    /* ============================================================
     *  MÉTODOS NOVOS - RF24 e RF25  (delegam no repositório)
     * ============================================================ */

    public async Task<IReadOnlyList<HoursPerDayDto>> GetHoursPerDayAsync(int userId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        var tarefas = await ctx.Tarefa
            .Where(t => t.Responsavel == userId &&
                        !t.IsDeleted &&
                        t.DataFim != null &&
                        t.Estado == "Concluído")
            .ToListAsync();

        var resultado = tarefas
            .GroupBy(t => DateOnly.FromDateTime(t.DataFim!.Value.Date))
            .Select(g => new HoursPerDayDto(
                g.Key,
                Math.Round(g.Sum(t => (t.DataFim!.Value - t.DataInicio).TotalHours), 2)
            ))
            .OrderBy(x => x.Date)
            .ToList();

        return resultado;
    }


    public Task<IReadOnlyList<HoursPerMonthDto>> GetHoursPerMonthAsync(int userId) =>
        _repo.GetHoursPerMonthAsync(userId);

    public Task<IReadOnlyList<CostPerDayDto>> GetCostPerDayAsync(int userId) =>
        _repo.GetCostPerDayAsync(userId);

    public Task<IReadOnlyList<CostPerMonthDto>> GetCostPerMonthAsync(int userId) =>
        _repo.GetCostPerMonthAsync(userId);

    /* ============================================================
     *  RF23 – Relatório mensal detalhado de um utilizador
     * ============================================================ */

    public async Task<MonthlyUserReportDto> GetMonthlyUserReportAsync(int userId,
                                                                      int year,
                                                                      int month)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end   = start.AddMonths(1);

        var user = await ctx.Utilizador
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.IdUtilizador == userId &&
                                                      !u.IsDeleted)
                   ?? throw new KeyNotFoundException("Utilizador não encontrado.");

        var tasks = await ctx.Tarefa
            .Include(t => t.IdProjetos)                // projectos para nome/cliente/rate
            .AsNoTracking()
            .Where(t => t.Responsavel == userId &&
                        t.DataFim != null &&
                        t.DataFim >= start && t.DataFim < end &&
                        t.Estado == "Concluído" &&      // RF26
                        !t.IsDeleted)
            .ToListAsync();

        var dailyEntries = tasks
            .GroupBy(t => DateOnly.FromDateTime(t.DataFim!.Value.Date))
            .Select(g =>
            {
                var dayTasks = g.Select(t =>
                {
                    var hours = (t.DataFim!.Value - t.DataInicio).TotalHours;
                    var rate  = t.PrecoHora
                             ?? t.IdProjetos.FirstOrDefault()?.PrecoHora
                             ?? 0M;
                    return new DailyTaskDto(
                        t.IdTarefa,
                        t.Titulo,
                        t.IdProjetos.First().IdProjeto,
                        t.IdProjetos.First().Nome,
                        t.IdProjetos.First().NomeCliente,
                        hours,
                        (decimal)hours * rate
                    );
                }).ToList();

                var totalHours = dayTasks.Sum(d => d.Hours);
                var totalCost  = dayTasks.Sum(d => d.Cost);
                var habitual   = user.NumHoras ?? 8;              // RF27

                return new DailyReportEntryDto(
                    g.Key,
                    dayTasks,
                    Math.Round(totalHours, 2),
                    Math.Round(totalCost, 2),
                    totalHours > habitual
                );
            })
            .OrderBy(e => e.Day)
            .ToList();

        return new MonthlyUserReportDto(
            user.IdUtilizador,
            user.Nome,
            year,
            month,
            dailyEntries,
            Math.Round(dailyEntries.Sum(e => e.TotalHours), 2),
            Math.Round(dailyEntries.Sum(e => e.TotalCost), 2)
        );
    }

    /* ============================================================
     *  RF28 – Relatórios mensais consolidados por projecto/cliente
     * ============================================================ */

    public async Task<IEnumerable<ProjectMonthlyReportDto>> GetMonthlyProjectReportsAsync(int year,
                                                                                           int month)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end   = start.AddMonths(1);

        var tasks = await ctx.Tarefa
            .Include(t => t.IdProjetos)
            .Include(t => t.ResponsavelNavigation)
            .AsNoTracking()
            .Where(t => t.DataFim != null &&
                        t.DataFim >= start && t.DataFim < end &&
                        t.Estado == "Concluído" &&          // RF26
                        !t.IsDeleted)
            .ToListAsync();

        var reports = tasks
            .SelectMany(t => t.IdProjetos.Select(p => new { Task = t, Project = p }))
            .GroupBy(tp => tp.Project.IdProjeto)
            .Select(g =>
            {
                var project = g.First().Project;

                // — tarefas deste projecto
                var taskDtos = g.Select(tp =>
                {
                    var t     = tp.Task;
                    var hours = (t.DataFim!.Value - t.DataInicio).TotalHours;
                    var rate  = t.PrecoHora ?? project.PrecoHora ?? 0M;

                    return new DailyTaskDto(
                        t.IdTarefa,
                        t.Titulo,
                        project.IdProjeto,
                        project.Nome,
                        project.NomeCliente,
                        hours,
                        (decimal)hours * rate
                    );
                }).ToList();

                // — utilizadores envolvidos
                var users = g.GroupBy(tp => tp.Task.ResponsavelNavigation)
                             .Select(ug =>
                             {
                                 var hrs  = ug.Sum(tp => (tp.Task.DataFim!.Value - tp.Task.DataInicio)
                                                            .TotalHours);
                                 var rate = ug.First().Task.PrecoHora
                                          ?? project.PrecoHora
                                          ?? 0M;

                                 return new ProjectUserHoursDto(
                                     ug.Key.IdUtilizador,
                                     ug.Key.Nome,
                                     Math.Round(hrs, 2),
                                     Math.Round((decimal)hrs * rate, 2)
                                 );
                             }).ToList();

                return new ProjectMonthlyReportDto(
                    project.IdProjeto,
                    project.Nome,
                    project.NomeCliente,
                    year,
                    month,
                    taskDtos,
                    users,
                    Math.Round(taskDtos.Sum(t => t.Hours), 2),
                    Math.Round(taskDtos.Sum(t => t.Cost), 2)
                );
            })
            .OrderBy(r => r.ProjectName)
            .ToList();

        return reports;
    }
}
