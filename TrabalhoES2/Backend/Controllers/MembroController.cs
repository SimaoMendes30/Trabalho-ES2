using Backend.DTOs.Projeto;
using Backend.DTOs.Tarefas;
using Backend.DTOs.Membros;
using Backend.DTOs.Relatorios;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public sealed class MembroController : ControllerBase
{
    private readonly IMembroService _service;
    public MembroController(IMembroService service) => _service = service;

    [HttpGet("projeto/{projetoId:int}")]
    public async Task<IEnumerable<MembroDto>> PorProjeto(int projetoId) =>
        await _service.GetByProjetoAsync(projetoId);

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateMembroDto dto)
    {
        await _service.AddAsync(dto);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}