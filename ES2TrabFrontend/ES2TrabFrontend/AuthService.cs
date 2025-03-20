using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username = username, Password = password });
        return response.IsSuccessStatusCode;
    }
}