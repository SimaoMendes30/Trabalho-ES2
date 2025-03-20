namespace ES2TrabFrontend.Services;
using ES2TrabFrontend.Models;
using System.Net.Http.Json;
public class UtilizadorService
{
    private readonly HttpClient _httpClient;

    public UtilizadorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Utilizador>> GetUtilizadoresAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Utilizador>>("api/utilizadores");
    }

    public async Task<Utilizador> GetUtilizadorByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Utilizador>($"api/utilizadores/{id}");
    }

    public async Task<HttpResponseMessage> CreateUtilizadorAsync(Utilizador utilizador)
    {
        return await _httpClient.PostAsJsonAsync("api/utilizadores", utilizador);
    }

    public async Task<HttpResponseMessage> UpdateUtilizadorAsync(int id, Utilizador utilizador)
    {
        return await _httpClient.PutAsJsonAsync($"api/utilizadores/{id}", utilizador);
    }

    public async Task<HttpResponseMessage> DeleteUtilizadorAsync(int id)
    {
        return await _httpClient.DeleteAsync($"api/utilizadores/{id}");
    }
}
