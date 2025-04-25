namespace Backend.DTOs.Tarefas;

public sealed record StartTarefaDto(
    string Descricao,
    int Responsavel,
    int ProjetoId,
    decimal? PrecoHora);
