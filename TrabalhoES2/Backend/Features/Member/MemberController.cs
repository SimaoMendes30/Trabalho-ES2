using Backend.Domain.DTOs.Common;
using Backend.Domain.DTOs.Member;
using Backend.Features.Member.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Member;

[ApiController]
[Route("api/membros")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _service;

    public MemberController(IMemberService service)
    {
        _service = service;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<MemberDetailsDto>>> GetPaged([FromQuery] MemberFilterDto filter, int page = 1, int pageSize = 10)
    {
        var result = await _service.FilteredPagedAsync(filter, page, pageSize);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDetailsDto>>> GetAll([FromQuery] MemberFilterDto filter)
    {
        var result = await _service.FilteredListAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDetailsExtendedDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDetailsDto>> Create(MemberCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdUtilizador }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MemberUpdateDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    // ➡️ Enviar convite para Tarefa
    // POST /api/membros/tarefa/{taskId}
    [HttpPost("tarefa/{taskId}")]
    public async Task<ActionResult<MemberDetailsDto>> InviteToTask(
        int taskId, MemberCreateDto dto, [FromQuery] int projectId)
    {
        var result = await _service.InviteToTaskAsync(dto.IdUtilizador, taskId, projectId);
        return CreatedAtAction(nameof(GetById), new { id = result.IdMembro }, result);
    }


    // ➡️ Responder a convite para Tarefa
    [HttpPut("tarefa/{id}")]
    public async Task<IActionResult> RespondToTaskInvitation(int id, [FromQuery] bool accept)
    {
        await _service.RespondToTaskInvitationAsync(id, accept);
        return NoContent();
    }
    
    [HttpPut("projeto/{id}")]
    public async Task<IActionResult> RespondToProjectInvitation(int id, [FromQuery] bool accept)
    {
        await _service.RespondToProjectInvitationAsync(id, accept);
        return NoContent();
    }
    [HttpDelete("projeto/{id}/utilizador/{userId}")]
    public async Task<IActionResult> RemoveMemberFromProject(int id, int userId, [FromQuery] int currentUserId)
    {
        try
        {
            await _service.RemoveMemberFromProjectAsync(currentUserId, id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message); // ou: return StatusCode(403, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }


    [HttpDelete("tarefa/{id}/utilizador/{userId}")]
    public async Task<IActionResult> RemoveMemberFromTask(int id, int userId)
    {
        await _service.RemoveMemberFromTaskAsync(id, userId);
        return NoContent();
    }
    
    [HttpPost("projeto")]
    public async Task<ActionResult<MemberDetailsDto>> InviteToProject(
        [FromQuery] int currentUserId,
        [FromQuery] int userId,
        [FromQuery] int projectId)
    {
        var result = await _service.InviteToProjectAsync(currentUserId, userId, projectId);
        return CreatedAtAction(nameof(GetById), new { id = result.IdMembro }, result);
    }

    
    [HttpGet("pending/{userId}")]
    public async Task<ActionResult<IEnumerable<MemberDetailsDto>>> GetPendingInvitations(int userId)
    {
        var invites = await _service.GetPendingInvitationsAsync(userId);
        return Ok(invites);
    }
}
