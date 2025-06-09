using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.DTOs.Report;

public class ReportsService
{
    private readonly HttpClient _http;

    public ReportsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ReportHourDayDto>> GetHorasPorDiaAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<ReportHourDayDto>>(
            $"api/reports/users/{userId}/hours/day") ?? new();
    }

    public async Task<List<ReportHourMonthDto>> GetHorasPorMesAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<ReportHourMonthDto>>(
            $"api/reports/users/{userId}/hours/month") ?? new();
    }

    public async Task<List<ReportCostDayDto>> GetCustosPorDiaAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<ReportCostDayDto>>(
            $"api/reports/users/{userId}/cost/day") ?? new();
    }

    public async Task<List<ReportCostMonthDto>> GetCustosPorMesAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<ReportCostMonthDto>>(
            $"api/reports/users/{userId}/cost/month") ?? new();
    }

    public async Task<List<ReportDetailsDto>> GetRelatorioDetalhadoMensalAsync(int userId)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        var report = await _http.GetFromJsonAsync<MonthlyUserReportDto>(
            $"api/reports/users/{userId}/monthly?year={year}&month={month}");

        var details = new List<ReportDetailsDto>();

        if (report?.Entries != null)
        {
            foreach (var entry in report.Entries)
            {
                foreach (var task in entry.Tasks)
                {
                    details.Add(new ReportDetailsDto
                    {
                        Day = entry.Day,
                        Projeto = task.ProjectName,
                        TituloTarefa = task.Title,
                        Cliente = task.ClientName ?? "-",
                        Utilizadores = report.UserName,
                        Horas = task.Hours,
                        Custo = task.Cost
                    });
                }
            }
        }

        return details;
    }
}
