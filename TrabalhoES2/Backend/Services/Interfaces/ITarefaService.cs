namespace Backend.Services.Interfaces;

using Backend.DTOs.Tarefas;
using Backend.DTOs.Relatorios;

public interface ITarefaService
{
    Task<TarefaDto>              StartAsync(StartTarefaDto dto);
    Task<TarefaDto>              EndAsync(int id, EndTarefaDto dto);
    Task                         MoveAsync(int tarefaId, int projetoDestinoId);
    Task<IEnumerable<TarefaDto>> ListarEmCursoAsync(int utilizadorId);
    Task<IEnumerable<TarefaDto>> ListarConcluidasAsync(int utilizadorId, DateTime inicio, DateTime fim);
    Task<RelatorioMensalDto>     RelatorioMensalAsync(int utilizadorId, int ano, int mes);
    Task<IEnumerable<TarefaDto>> GetByProjetoIdAsync(int projetoId);
    Task<IEnumerable<TarefaDto>> GetByUtilizadorIdAsync(int utilizadorId);
}