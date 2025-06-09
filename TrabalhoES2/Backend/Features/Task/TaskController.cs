using Backend.Domain.DTOs.Common;
using Backend.Domain.DTOs.Task;
using Backend.Features.Task.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Task;

[ApiController]
[Route("api/tarefas")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<TaskDetailsDto>>> GetPaged([FromQuery] TaskFilterDto filter, int page = 1, int pageSize = 10)
    {
        var result = await _service.FilteredPagedAsync(filter, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDetailsExtendedDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDetailsDto>> Create(TaskCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdTarefa }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
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
    
    // 🔹 POST: /api/tarefas/{taskId}/associate/{projectId}
    [HttpPost("{taskId}/associate/{projectId}")]
    public async Task<IActionResult> AssociateToProject(int taskId, int projectId)
    {
        await _service.AssociateTaskToProjectAsync(taskId, projectId);
        return Ok(new { Message = $"Tarefa {taskId} associada ao Projeto {projectId} com sucesso!" });
    }
    
    [HttpGet("projeto/{projetoId}")]
    public async Task<ActionResult<IEnumerable<TaskDetailsDto>>> GetByProjetoId(int projetoId)
    {
        var tarefas = await _service.GetByProjetoIdAsync(projetoId);
        return Ok(tarefas);
    }
    
    [HttpPut("{id}/concluir")]
    public async Task<IActionResult> Concluir(int id, [FromQuery] int userId)
    {
        await _service.ConcluirTarefaAsync(id, userId);
        return NoContent();
    }
    
    
    [HttpDelete("{taskId}/disassociate/{projectId}")]
    public async Task<IActionResult> DisassociateFromProject(int userId,int taskId, int projectId)
    {
        await _service.DisassociateTaskFromProjectAsync(userId, taskId, projectId);
        return Ok(new { Message = $"Tarefa {taskId} desassociada do Projeto {projectId} com sucesso!" });
    }
    
    
    // GET api/tarefas/em-curso?userId=123
    [HttpGet("em-curso")]
    public async Task<ActionResult<InProgressResultDto>> GetInProgress([FromQuery] int userId)
    {
        var result = await _service.GetInProgressAsync(userId);
        return Ok(result);
    }
        
    /*
    // GET api/tarefas/concluidas?from=2025-01-01T00:00:00Z&to=2025-03-01T00:00:00Z
    [HttpGet("concluidas")]
    public async Task<ActionResult<IEnumerable<TaskDetailsDto>>> GetCompleted(
        [FromQuery] int userId,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to)
    {
        var result = await _service.GetCompletedInIntervalAsync(userId, from, to);
        return Ok(result);
    }
    */

}