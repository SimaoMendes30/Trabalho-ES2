using Frontend.DTO_s;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class TarefaService
    {
        private readonly HttpClient _httpClient;

        public TarefaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Obter todas as tarefas
        public async Task<List<TarefaDTO>> GetTarefasAsync()
        {
            var response = await _httpClient.GetAsync("api/tarefa");

            if (response.IsSuccessStatusCode)
            {
                var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaDTO>>();
                return tarefas ?? new List<TarefaDTO>();
            }

            return new List<TarefaDTO>();
        }

        // Obter uma tarefa pelo ID
        public async Task<TarefaDTO> GetTarefaByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/tarefa/{id}");

            if (response.IsSuccessStatusCode)
            {
                var tarefa = await response.Content.ReadFromJsonAsync<TarefaDTO>();
                return tarefa;
            }

            return null;
        }

        // Criar uma nova tarefa
        public async Task<TarefaDTO> CreateTarefaAsync(TarefaDTO tarefaDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/tarefa", tarefaDto);

            if (response.IsSuccessStatusCode)
            {
                var newTarefa = await response.Content.ReadFromJsonAsync<TarefaDTO>();
                return newTarefa;
            }

            return null;
        }

        // Atualizar uma tarefa
        public async Task<bool> UpdateTarefaAsync(int id, TarefaDTO tarefaDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/tarefa/{id}", tarefaDto);

            return response.IsSuccessStatusCode;
        }

        // Deletar uma tarefa
        public async Task<bool> DeleteTarefaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/tarefa/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
