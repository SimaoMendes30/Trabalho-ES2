namespace Backend.DTOs.Relatorios;

public sealed class DiaRelatorioDto
{
    public DateTime Dia { get; init; }
    public decimal TotalHoras { get; init; }
    public decimal TotalCusto { get; init; }
    public List<ProjetoDiaDto> Projetos { get; init; } = new();
}
