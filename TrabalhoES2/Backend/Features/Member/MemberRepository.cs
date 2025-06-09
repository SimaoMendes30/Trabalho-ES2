using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Backend.Domain.DTOs.Common;
using Backend.Domain.DTOs.Member;
using Backend.Domain.Patterns.Specifications;
using Backend.Features.Member.Interfaces;

namespace Backend.Features.Member;

public sealed class MemberRepository : IMemberRepository
{
    private readonly IDbContextFactory<SgscDbContext> _factory;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(IDbContextFactory<SgscDbContext> factory, ILogger<MemberRepository> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<MemberEntity?> GetByIdAsync(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Membro
            .Include(m => m.IdUserEntityNavigation)
            .Include(m => m.IdProjectEntityNavigation)
            .FirstOrDefaultAsync(m => m.IdMembro == id);
    }

    public async System.Threading.Tasks.Task AddAsync(MemberEntity member)
    {
        Validate(member);

        await using var ctx = _factory.CreateDbContext();
        ctx.Membro.Add(member);
        await ctx.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task UpdateAsync(MemberEntity member)
    {
        Validate(member);

        await using var ctx = _factory.CreateDbContext();
        ctx.Membro.Update(member);
        await ctx.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        var entity = await ctx.Membro.FindAsync(id);
        if (entity is null) return;

        ctx.Membro.Remove(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<MemberEntity>> FilteredListAsync(MemberFilterDto filter)
    {
        await using var ctx = _factory.CreateDbContext();
        var spec = new MemberByFilterSpec(filter);
        return await ctx.Membro
            .Where(spec.ToExpression())
            .Include(m => m.IdUserEntityNavigation)
            .Include(m => m.IdProjectEntityNavigation)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<PagedResult<MemberEntity>> FilteredPagedAsync(MemberFilterDto filter, int page, int pageSize)
    {
        await using var ctx = _factory.CreateDbContext();
        var spec = new MemberByFilterSpec(filter);

        var query = ctx.Membro
            .Where(spec.ToExpression())
            .Include(m => m.IdUserEntityNavigation)
            .Include(m => m.IdProjectEntityNavigation)
            .AsNoTracking();

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<MemberEntity>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    private void Validate(MemberEntity member)
    {
        var allowedInviteStatuses = new[] { "Pendente", "Aceite", "Recusado" };
        if (!allowedInviteStatuses.Contains(member.EstadoConvite))
            throw new ValidationException("EstadoConvite inválido. Deve ser: Pendente, Aceite ou Recusado.");

        var allowedActivityStatuses = new[] { "Ativo", "Inativo" };
        if (!string.IsNullOrEmpty(member.EstadoAtividade) &&
            !allowedActivityStatuses.Contains(member.EstadoAtividade))
        {
            throw new ValidationException("EstadoAtividade inválido. Deve ser: Ativo ou Inativo.");
        }
    }
    
    public async System.Threading.Tasks.Task RemoveFromProjectAsync(int projectId, int userId)
    {
        await using var ctx = _factory.CreateDbContext();

        var member = await ctx.Membro
            .FirstOrDefaultAsync(m => m.IdProjeto == projectId && m.IdUtilizador == userId);

        if (member != null)
        {
            member.EstadoAtividade = "Inativo";
            member.EstadoConvite = "Recusado";
            member.DataEstado = DateTimeOffset.UtcNow;

            ctx.Membro.Update(member);
            await ctx.SaveChangesAsync();
        }

        // Remover o membro de todas as tarefas do projeto
        var tasks = await ctx.Tarefa
            .Where(t => t.IdProjetos.Any(p => p.IdProjeto == projectId))
            .Include(t => t.IdUtilizadors)
            .ToListAsync();

        foreach (var task in tasks)
        {
            var userToRemove = task.IdUtilizadors.FirstOrDefault(u => u.IdUtilizador == userId);
            if (userToRemove != null)
            {
                task.IdUtilizadors.Remove(userToRemove);
            }
        }

        await ctx.SaveChangesAsync();
    }



    public async System.Threading.Tasks.Task RemoveFromTaskAsync(int taskId, int userId)
    {
        await using var ctx = _factory.CreateDbContext();
    
        var task = await ctx.Tarefa
            .Include(t => t.IdUtilizadors)
            .FirstOrDefaultAsync(t => t.IdTarefa == taskId);

        if (task != null)
        {
            var user = task.IdUtilizadors.FirstOrDefault(u => u.IdUtilizador == userId);

            if (user != null)
            {
                // Remover o utilizador da coleção
                task.IdUtilizadors.Remove(user);

                // Atualizar a coleção no Entity Framework
                ctx.Entry(task).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
    
    public async Task<bool> ExistsAsync(int projectId, int userId)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Membro.AnyAsync(m => m.IdProjeto == projectId && m.IdUtilizador == userId);
    }
    
    public async Task<IEnumerable<MemberEntity>> GetPendingInvitationsAsync(int userId)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Membro
            .Include(m => m.IdUserEntityNavigation)
            .Include(m => m.IdProjectEntityNavigation)
            .Where(m => m.IdUtilizador == userId && m.EstadoConvite == "Pendente")
            .ToListAsync();
    }
}
