namespace Backend.Domain.Builders;
using Backend.Models;
using Backend.DTOs.Relatorios;
public sealed class RelatorioMensalBuilder
{
    private readonly List<Tarefa> _tarefas = new();

    public RelatorioMensalBuilder Add(IEnumerable<Tarefa> tarefas)
    {
        _tarefas.AddRange(tarefas);
        return this;
    }

    public RelatorioMensalDto Build()
    {
        // Filtra tarefas que têm DataFim e DataHoraInicio definidos
        var dias = _tarefas.Where(t => t.DataFim != null && t.DataHoraInicio != null)
            .GroupBy(t => t.DataFim!.Value)          // DataFim é DateOnly ➜ group by próprio valor (dia)
            .Select(g => new DiaRelatorioDto
            {
                Dia = g.Key.ToDateTime(TimeOnly.MinValue).Date,
                TotalHoras = g.Sum(t =>
                    (decimal)((t.DataFim!.Value.ToDateTime(TimeOnly.MinValue) - t.DataHoraInicio!.Value).TotalHours)),
                TotalCusto = g.Sum(t =>
                    (t.PrecoHora ?? 0) * (decimal)((t.DataFim!.Value.ToDateTime(TimeOnly.MinValue) - t.DataHoraInicio!.Value).TotalHours)),
                Projetos = g.GroupBy(t => t.IdProjetos.FirstOrDefault()?.Nome ?? "Sem Projeto")
                    .Select(p => new ProjetoDiaDto
                    {
                        NomeProjeto = p.Key,
                        Horas = p.Sum(t => (decimal)((t.DataFim!.Value.ToDateTime(TimeOnly.MinValue) - t.DataHoraInicio!.Value).TotalHours)),
                        Custo = p.Sum(t => (t.PrecoHora ?? 0) * (decimal)((t.DataFim!.Value.ToDateTime(TimeOnly.MinValue) - t.DataHoraInicio!.Value).TotalHours))
                    }).ToList()
            }).ToList();

        return new RelatorioMensalDto
        {
            Dias = dias,
            TotalHorasMes = dias.Sum(d => d.TotalHoras),
            TotalCustoMes = dias.Sum(d => d.TotalCusto)
        };
    }
}