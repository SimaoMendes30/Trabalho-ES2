namespace ES2TrabFrontend.Services;
using ES2TrabFrontend.Models;
using System.Net.Http.Json;
public class TarefaService
{
    private readonly HttpClient _httpClient;

    public TarefaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Tarefa>> GetTarefasAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Tarefa>>("api/tarefas");
    }

    public async Task<Tarefa> GetTarefaByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Tarefa>($"api/tarefas/{id}");
    }

    public async Task<HttpResponseMessage> CreateTarefaAsync(Tarefa tarefa)
    {
        return await _httpClient.PostAsJsonAsync("api/tarefas", tarefa);
    }

    public async Task<HttpResponseMessage> UpdateTarefaAsync(int id, Tarefa tarefa)
    {
        return await _httpClient.PutAsJsonAsync($"api/tarefas/{id}", tarefa);
    }

    public async Task<HttpResponseMessage> DeleteTarefaAsync(int id)
    {
        return await _httpClient.DeleteAsync($"api/tarefas/{id}");
    }
}
