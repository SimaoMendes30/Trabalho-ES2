using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.DTO_s;

namespace Frontend.Services
{
    public class MembroService
    {
        private readonly ApiService _apiService;

        public MembroService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Obter todos os membros
        public async Task<List<MembroDTO>> GetMembrosAsync()
        {
            return await _apiService.GetAsync<List<MembroDTO>>("api/membro");
        }

        // Obter um membro específico pelo ID
        public async Task<MembroDTO> GetMembroByIdAsync(int id)
        {
            return await _apiService.GetAsync<MembroDTO>($"api/membro/{id}");
        }

        // Criar um novo membro
        public async Task<MembroDTO> CreateMembroAsync(MembroDTO membroDTO)
        {
            return await _apiService.PostAsync("api/membro", membroDTO);
        }

        // Atualizar um membro
        public async Task UpdateMembroAsync(int id, MembroDTO membroDTO)
        {
            await _apiService.PutAsync($"api/membro/{id}", membroDTO);
        }

        // Deletar um membro
        public async Task DeleteMembroAsync(int id)
        {
            await _apiService.DeleteAsync($"api/membro/{id}");
        }
    }
}