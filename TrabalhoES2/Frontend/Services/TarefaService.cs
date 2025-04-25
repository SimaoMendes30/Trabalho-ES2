using Frontend.DTOs.Tarefas;
using System.Net.Http.Json;

namespace Frontend.Services;

public class TarefaService : ApiService
{
    public TarefaService(IHttpClientFactory f) : base(f) { }

    public async Task<TarefaDto?> StartAsync(StartTarefaDto dto)
    {
        var rsp = await PostAsync("api/tarefa/iniciar", dto);
        return rsp.IsSuccessStatusCode
            ? await rsp.Content.ReadFromJsonAsync<TarefaDto>()
            : null;
    }

    public async Task<TarefaDto?> EndAsync(int id, EndTarefaDto dto)
    {
        var rsp = await PostAsync($"api/tarefa/{id}/terminar", dto);
        return rsp.IsSuccessStatusCode ? await rsp.Content.ReadFromJsonAsync<TarefaDto>() : null;
    }

    public Task<bool> MoveAsync(int tarefaId, int projetoDestinoId) =>
        PutAsync($"api/tarefa/{tarefaId}/mover/{projetoDestinoId}", "").ContinueWith(t => t.Result.IsSuccessStatusCode);

    public async Task<IEnumerable<TarefaDto>?> GetByProjetoIdAsync(int projetoId)
    {
        return await GetAsync<IEnumerable<TarefaDto>>($"api/tarefa/projeto/{projetoId}");
    }

    public async Task<IEnumerable<TarefaDto>?> ListByUserAsync(int userId)
    {
        return await GetAsync<IEnumerable<TarefaDto>>($"api/tarefa/utilizador/{userId}");
    }
    
    public async Task<bool> DeleteAsync(int tarefaId)
    {
        var response = await base.DeleteAsync($"api/tarefa/{tarefaId}");
        return response.IsSuccessStatusCode;
    }

}