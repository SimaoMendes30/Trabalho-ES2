using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.DTO_s;

namespace Frontend.Services
{
    public class ProjetoService
    {
        private readonly ApiService _apiService;

        public ProjetoService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ProjetoDTO>> GetProjetosAsync()
        {
            return await _apiService.GetAsync<List<ProjetoDTO>>("api/projeto");
        }

        public async Task<ProjetoDTO> GetProjetoByIdAsync(int id)
        {
            return await _apiService.GetAsync<ProjetoDTO>($"api/projeto/{id}");
        }

        public async Task<ProjetoDTO> CreateProjetoAsync(ProjetoDTO projetoDTO)
        {
            return await _apiService.PostAsync("api/projeto", projetoDTO);
        }

        public async Task UpdateProjetoAsync(int id, ProjetoDTO projetoDTO)
        {
            await _apiService.PutAsync($"api/projeto/{id}", projetoDTO);
        }

        public async Task DeleteProjetoAsync(int id)
        {
            await _apiService.DeleteAsync($"api/projeto/{id}");
        }
    }
}