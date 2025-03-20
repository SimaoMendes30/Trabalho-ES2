using System.Net.Http.Json;
using ES2TrabFrontend.Models;

namespace ES2TrabFrontend.Services;

public class ProjetoService
{
    private readonly HttpClient _httpClient;

    public ProjetoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProjetoDTO>> GetProjetosAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProjetoDTO>>("api/projetos");
    }

    public async Task<ProjetoDTO> GetProjetoByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ProjetoDTO>($"api/projetos/{id}");
    }

    public async Task<HttpResponseMessage> CreateProjetoAsync(ProjetoDTO projeto)
    {
        return await _httpClient.PostAsJsonAsync("api/projetos", projeto);
    }

    public async Task<HttpResponseMessage> UpdateProjetoAsync(int id, ProjetoDTO projeto)
    {
        return await _httpClient.PutAsJsonAsync($"api/projetos/{id}", projeto);
    }

    public async Task<HttpResponseMessage> DeleteProjetoAsync(int id)
    {
        return await _httpClient.DeleteAsync($"api/projetos/{id}");
    }
}