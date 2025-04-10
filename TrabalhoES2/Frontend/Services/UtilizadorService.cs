using Frontend.DTO_s;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class UtilizadorService
    {
        private readonly HttpClient _httpClient;

        public UtilizadorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Obter todos os utilizadores
        public async Task<List<UtilizadorDTO>> GetUtilizadoresAsync()
        {
            var response = await _httpClient.GetAsync("api/utilizador");

            if (response.IsSuccessStatusCode)
            {
                var utilizadores = await response.Content.ReadFromJsonAsync<List<UtilizadorDTO>>();
                return utilizadores ?? new List<UtilizadorDTO>();
            }

            return new List<UtilizadorDTO>();
        }

        // Obter um utilizador pelo ID
        public async Task<UtilizadorDTO> GetUtilizadorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/utilizador/{id}");

            if (response.IsSuccessStatusCode)
            {
                var utilizador = await response.Content.ReadFromJsonAsync<UtilizadorDTO>();
                return utilizador;
            }

            return null;
        }

        // Criar um novo utilizador
        public async Task<UtilizadorDTO> CreateUtilizadorAsync(UtilizadorDTO utilizadorDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/utilizador", utilizadorDto);

            if (response.IsSuccessStatusCode)
            {
                var newUtilizador = await response.Content.ReadFromJsonAsync<UtilizadorDTO>();
                return newUtilizador;
            }

            return null;
        }

        // Atualizar um utilizador
        public async Task<bool> UpdateUtilizadorAsync(int id, UtilizadorDTO utilizadorDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/utilizador/{id}", utilizadorDto);

            return response.IsSuccessStatusCode;
        }

        // Deletar um utilizador
        public async Task<bool> DeleteUtilizadorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/utilizador/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
