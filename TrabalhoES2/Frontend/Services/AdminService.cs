using System.Net.Http.Json;
using Frontend.DTOs.User;

namespace Frontend.Services
{
    public class AdminService
    {
        private readonly HttpClient _http;
        private const string AdminBase = "api/admin";

        public AdminService(HttpClient http) => _http = http;

        /// <summary>
        /// GET api/admin/users
        /// </summary>
        public async Task<List<UserDetailsDto>> GetAllUsersAsync() =>
            await _http.GetFromJsonAsync<List<UserDetailsDto>>($"{AdminBase}/users")
            ?? new();

        /// <summary>
        /// GET api/admin/users/{id}
        /// </summary>
        public async Task<UserDetailsDto?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<UserDetailsDto>($"{AdminBase}/users/{id}");
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// PUT api/admin/users/{id}
        /// </summary>
        public async Task UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var resp = await _http.PutAsJsonAsync($"{AdminBase}/users/{id}", dto);
            resp.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// DELETE api/admin/users/{id}
        /// </summary>
        public async Task DeleteUserAsync(int id)
        {
            var resp = await _http.DeleteAsync($"{AdminBase}/users/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}