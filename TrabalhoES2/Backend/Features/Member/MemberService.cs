using AutoMapper;
using Backend.Models;
using Backend.Domain.DTOs.Common;
using Backend.Domain.DTOs.Member;
using Backend.Features.Member.Interfaces;
using Backend.Domain.Patterns.Factories;
using Backend.Features.Project.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Member;

public sealed class MemberService : IMemberService
{
    private readonly IMemberRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<MemberService> _logger;
    private readonly IDbContextFactory<SgscDbContext> _factory;
    private readonly IProjectRepository _repoProject;

    public MemberService(IMemberRepository repo, IMapper mapper, ILogger<MemberService> logger,IDbContextFactory<SgscDbContext> factory,  IProjectRepository repoProject)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repoProject = repoProject;
    }

    public async Task<MemberDetailsDto> CreateAsync(MemberCreateDto dto)
    {
        var entity = MemberFactory.Create(
            idProjeto: dto.IdProjeto,
            idUtilizador: dto.IdUtilizador,
            estadoConvite: dto.EstadoConvite,
            estadoAtividade: dto.EstadoAtividade,
            dataConvite: dto.DataConvite,
            dataEstado: dto.DataEstado
            
        );

        await _repo.AddAsync(entity);
        return _mapper.Map<MemberDetailsDto>(entity);
    }

    public async Task<IEnumerable<MemberDetailsDto>> FilteredListAsync(MemberFilterDto filter)
    {
        var list = await _repo.FilteredListAsync(filter);
        return _mapper.Map<IEnumerable<MemberDetailsDto>>(list);
    }

    public async Task<PagedResult<MemberDetailsDto>> FilteredPagedAsync(MemberFilterDto filter, int page, int pageSize)
    {
        var result = await _repo.FilteredPagedAsync(filter, page, pageSize);
        return new PagedResult<MemberDetailsDto>
        {
            Items = _mapper.Map<IEnumerable<MemberDetailsDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<MemberDetailsExtendedDto> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"Membro {id} não encontrado");

        return _mapper.Map<MemberDetailsExtendedDto>(entity);
    }

    public async System.Threading.Tasks.Task UpdateAsync(int id, MemberUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"Membro {id} não encontrado");

        _mapper.Map(dto, entity);
        await _repo.UpdateAsync(entity);
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
    }
    
    public async Task<MemberDetailsDto> InviteToTaskAsync(int userId, int taskId, int projectId)
    {
        await using var ctx = _factory.CreateDbContext();

        var project = await ctx.Projeto.FindAsync(projectId)
                      ?? throw new KeyNotFoundException($"Projeto {projectId} não encontrado");

        var task = await ctx.Tarefa
            .Include(t => t.IdProjetos)
            .Include(t => t.IdUtilizadors)
            .FirstOrDefaultAsync(t => t.IdTarefa == taskId);

        if (task == null)
            throw new KeyNotFoundException($"Tarefa {taskId} não encontrada");

        if (!task.IdProjetos.Any(p => p.IdProjeto == projectId))
            throw new InvalidOperationException($"Tarefa {taskId} não pertence ao projeto {projectId}");

        // Verificar se já está na tarefa
        if (!task.IdUtilizadors.Any(u => u.IdUtilizador == userId))
        {
            var user = await ctx.Utilizador.FindAsync(userId)
                       ?? throw new KeyNotFoundException($"Utilizador {userId} não encontrado");

            task.IdUtilizadors.Add(user);
            await ctx.SaveChangesAsync();
        }

        // Opcional: retornar um DTO "fictício" para a associação à tarefa
        return new MemberDetailsDto
        {
            IdProjeto = projectId,
            IdUtilizador = userId,
            EstadoAtividade = "Ativo",
            EstadoConvite = "Aceite",
            DataConvite = DateTimeOffset.UtcNow
        };
    }
    public async Task<bool> RespondToTaskInvitationAsync(int memberId, bool accept)
    {
        var memberEntity = await _repo.GetByIdAsync(memberId)
                           ?? throw new KeyNotFoundException($"Convite {memberId} não encontrado");

        memberEntity.EstadoConvite  = accept ? "Aceite" : "Recusado";
        memberEntity.EstadoAtividade = accept ? "Ativo"  : "Inativo";
        memberEntity.DataEstado      = DateTimeOffset.UtcNow;

        if (accept)
        {
            await using var ctx = _factory.CreateDbContext();

            // ⚠️ Aqui estava o erro. Estavas a procurar pela ID do Projeto em vez de pela ID da Tarefa.
            var task = await ctx.Tarefa
                .Include(t => t.IdUtilizadors)
                .FirstOrDefaultAsync(t => t.IdTarefa == memberEntity.IdProjeto); // Isto estava errado!

            if (task != null && !task.IdUtilizadors.Any(u => u.IdUtilizador == memberEntity.IdUtilizador))
            {
                var user = await ctx.Utilizador.FindAsync(memberEntity.IdUtilizador);
                if (user != null) task.IdUtilizadors.Add(user);
            }

            await ctx.SaveChangesAsync();
        }

        await _repo.UpdateAsync(memberEntity);
        return true;
    }

    
    public async Task<bool> RespondToProjectInvitationAsync(int memberId, bool accept)
    {
        var memberEntity = await _repo.GetByIdAsync(memberId)
                           ?? throw new KeyNotFoundException($"Convite {memberId} não encontrado");

        memberEntity.EstadoConvite = accept ? "Aceite" : "Recusado";
        memberEntity.EstadoAtividade = accept ? "Ativo" : "Inativo";
        memberEntity.DataEstado = DateTimeOffset.UtcNow;

        await _repo.UpdateAsync(memberEntity);

        return true;
    }
    public async System.Threading.Tasks.Task RemoveMemberFromProjectAsync(int currentUserId, int projectId, int userId)
    {
        var project = await _repoProject.GetByIdAsync(projectId)
                      ?? throw new KeyNotFoundException($"Projeto {projectId} não encontrado");

        if (project.Responsavel != currentUserId)
            throw new UnauthorizedAccessException("Apenas o responsável do projeto pode remover membros.");

        await _repo.RemoveFromProjectAsync(projectId, userId);
    }
    public async System.Threading.Tasks.Task RemoveMemberFromTaskAsync(int taskId, int userId)
    {
        await using var ctx = _factory.CreateDbContext();

        var task = await ctx.Tarefa
            .Include(t => t.IdProjetos)
            .Include(t => t.IdUtilizadors)
            .FirstOrDefaultAsync(t => t.IdTarefa == taskId);

        if (task == null)
            throw new KeyNotFoundException($"Tarefa {taskId} não encontrada");

        // Encontrar projeto responsável (assumindo que é 1 para 1)
        var projeto = task.IdProjetos.FirstOrDefault();

        if (projeto == null)
            throw new InvalidOperationException("A tarefa não está associada a nenhum projeto");

        // Validação de permissão deve ser feita no controller ou service antes de chamar este método.

        var user = task.IdUtilizadors.FirstOrDefault(u => u.IdUtilizador == userId);

        if (user != null)
        {
            task.IdUtilizadors.Remove(user);
            await ctx.SaveChangesAsync();
        }
    }
    public async Task<MemberDetailsDto> InviteToProjectAsync(int currentUserId, int userId, int projectId)
    {
        var project = await _repoProject.GetByIdAsync(projectId)
                      ?? throw new KeyNotFoundException($"Projeto {projectId} não encontrado");

        if (project.Responsavel != currentUserId)
            throw new UnauthorizedAccessException("Apenas o responsável do projeto pode convidar membros.");

        if (await _repo.ExistsAsync(projectId, userId))
            throw new InvalidOperationException("O utilizador já foi convidado ou faz parte do projeto.");

        var entity = MemberFactory.Create(projectId, userId, "Pendente", "Inativo", DateTimeOffset.UtcNow);
        await _repo.AddAsync(entity);
        return _mapper.Map<MemberDetailsDto>(entity);
    }


    public async Task<IEnumerable<MemberDetailsDto>> GetPendingInvitationsAsync(int userId)
    {
        var pendingInvites = await _repo.GetPendingInvitationsAsync(userId);
        return _mapper.Map<IEnumerable<MemberDetailsDto>>(pendingInvites);
    }
    
}