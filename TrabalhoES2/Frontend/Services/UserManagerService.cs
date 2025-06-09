using System.Net.Http.Json;
using Frontend.DTOs.User;

namespace Frontend.Services;

public class UserManagerService
{
    private readonly HttpClient _http;
    public UserManagerService(HttpClient http) => _http = http;

    public async Task<List<UserDetailsDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<UserDetailsDto>>("api/usermanager/all") ?? new();

    // 🔻 Troca Guid → int
    public async Task UpdateRoleAsync(int userId, string newRole)
    {
        var resp = await _http.PutAsJsonAsync(
            $"api/usermanager/update-role/{userId}",
            new { role = newRole });
        resp.EnsureSuccessStatusCode();
    }
}