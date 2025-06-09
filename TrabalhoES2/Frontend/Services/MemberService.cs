using Frontend.DTOs.Common;
using System.Net.Http.Json;
using Frontend.DTOs.Member;

namespace Frontend.Services;

public class MemberService : ApiService
{
    public MemberService(IHttpClientFactory f) : base(f) { }

    public async Task<IEnumerable<MemberDetailsDto>> GetAllAsync(MemberFilterDto? filter = null)
    {
        var result = await GetAsync<IEnumerable<MemberDetailsDto>>("api/membros", filter);
        return result ?? throw new Exception("Erro ao obter a lista de membros.");
    }

    public async Task<PagedResult<MemberDetailsDto>> GetPagedAsync(MemberFilterDto? filter = null, int page = 1, int pageSize = 10)
    {
        var query = new
        {
            page,
            pageSize,
            filter?.IdMembro,
            filter?.IdUtilizador,
            filter?.IdProjeto,
            filter?.DataConviteAte,
            filter?.DataConviteDe,
            filter?.DataEstadoAte,
            filter?.DataEstadoDe,
            filter?.EstadoConvite,
            filter?.EstadoAtividade
        };
        
        var result = await GetAsync<PagedResult<MemberDetailsDto>>("api/membros/paged", query);
    
        if (result == null)
        {
            throw new Exception("Erro ao obter membros paginados.");
        }

        return result;
    }

    public async Task<MemberDetailsExtendedDto> GetByIdAsync(int id)
    {
        var result = await GetAsync<MemberDetailsExtendedDto>($"api/membros/{id}");
        return result ?? throw new Exception($"Membro com ID {id} não encontrado.");
    }

    public async Task<MemberDetailsDto> CreateAsync(MemberCreateDto dto)
    {
        var response = await PostAsync("api/membros", dto);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao criar o membro.");

        var data = await response.Content.ReadFromJsonAsync<MemberDetailsDto>();
        return data ?? throw new Exception("Resposta inválida ao criar o membro.");
    }

    public async Task<bool> UpdateAsync(int id, MemberUpdateDto dto)
    {
        var response = await PutAsync($"api/membros/{id}", dto);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar o membro com ID {id}.");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await DeleteRequestAsync($"api/membros/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao eliminar o membro com ID {id}.");
        return true;
    }
    
    public async Task<MemberDetailsDto> InviteToTaskAsync(int userId, int taskId, int projectId)
    {
        var dto = new MemberCreateDto
        {
            IdUtilizador = userId,
            IdProjeto = projectId,
            IdTarefa = taskId,
            EstadoConvite = "Pendente",
            EstadoAtividade = "Inativo",
            DataConvite = DateTimeOffset.UtcNow
        };

        var response = await PostAsync("api/membros/tarefa", dto);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao enviar convite para tarefa.");

        var data = await response.Content.ReadFromJsonAsync<MemberDetailsDto>();
        return data ?? throw new Exception("Erro ao interpretar a resposta do convite.");
    }

    // Responder ao convite para tarefa
    public async Task<bool> RespondToTaskInvitationAsync(int id, bool accept)
    {
        var response = await PutAsync($"api/membros/tarefa/{id}?accept={accept}", new { });
    
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao responder ao convite para tarefa: {errorMessage}");
        }

        return true;
    }

    // ✅ **Novo método para enviar convite para Projeto**
    public async Task<MemberDetailsDto> InviteToProjectAsync(int currentUserId, int userId, int projectId)
    {
        var response = await PostAsync($"api/membros/projeto?currentUserId={currentUserId}&userId={userId}&projectId={projectId}", new { });
    
        if (!response.IsSuccessStatusCode)
            throw new Exception("Você não tem permissões para convidar outros utilizadores!");

        var data = await response.Content.ReadFromJsonAsync<MemberDetailsDto>();
        return data ?? throw new Exception("Erro ao interpretar a resposta do convite.");
    }


    // Responder ao convite para projeto
    public async Task<bool> RespondToProjectInvitationAsync(int id, bool accept)
    {
        var response = await PutAsync($"api/membros/projeto/{id}?accept={accept}", new { });
    
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao responder ao convite para projeto: {errorMessage}");
        }

        return true;
    }
    
    public async Task<IEnumerable<MemberDetailsDto>> GetPendingInvitationsAsync(int userId)
    {
        var result = await GetAsync<IEnumerable<MemberDetailsDto>>($"api/membros/pending/{userId}");
        return result ?? new List<MemberDetailsDto>();
    }

    public async Task<IEnumerable<MemberDetailsExtendedDto>> GetAllExtendedAsync(MemberFilterDto? filter = null)
    {
        var result = await GetAsync<IEnumerable<MemberDetailsExtendedDto>>("api/membros/extended", filter);
        return result ?? new List<MemberDetailsExtendedDto>();
    }
    public async Task<bool> RemoveMemberFromProjectAsync(int memberId, int userId, int currentUserId)
    {
        var url = $"api/membros/projeto/{memberId}/utilizador/{userId}?currentUserId={currentUserId}";
        var response = await DeleteRequestAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao remover membro: {error}");
        }

        return true;
    }
    
    public async Task<bool> RemoveMemberFromTaskAsync(int taskId, int userId)
    {
        var url = $"api/membros/tarefa/{taskId}/utilizador/{userId}";
        var response = await DeleteRequestAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao remover membro da tarefa: {error}");
        }

        return true;
    }
}
