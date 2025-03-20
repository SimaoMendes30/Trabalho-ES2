namespace ES2TrabFrontend.Services;
using ES2TrabFrontend.Models;
using System.Net.Http.Json;
public class MembroService
{
    private readonly HttpClient _httpClient;

    public MembroService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Membro>> GetMembrosAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Membro>>("api/membros");
    }

    public async Task<Membro> GetMembroByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Membro>($"api/membros/{id}");
    }

    public async Task<HttpResponseMessage> CreateMembroAsync(Membro membro)
    {
        return await _httpClient.PostAsJsonAsync("api/membros", membro);
    }

    public async Task<HttpResponseMessage> UpdateMembroAsync(int id, Membro membro)
    {
        return await _httpClient.PutAsJsonAsync($"api/membros/{id}", membro);
    }

    public async Task<HttpResponseMessage> DeleteMembroAsync(int id)
    {
        return await _httpClient.DeleteAsync($"api/membros/{id}");
    }
}
