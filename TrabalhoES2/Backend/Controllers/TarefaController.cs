using Backend.DTOs.Projeto;
using Backend.DTOs.Tarefas;
using Backend.DTOs.Membros;
using Backend.DTOs.Relatorios;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
    private readonly ITarefaService _service;
    public TarefaController(ITarefaService service) => _service = service;

    [HttpPost("iniciar")]
    public async Task<ActionResult<TarefaDto>> Iniciar([FromBody] StartTarefaDto dto) => Ok(await _service.StartAsync(dto));

    [HttpPost("{id:int}/terminar")]
    public async Task<ActionResult<TarefaDto>> Terminar(int id, [FromBody] EndTarefaDto dto) => Ok(await _service.EndAsync(id, dto));

    [HttpPatch("{id:int}/mover/{destinoId:int}")]
    public async Task<IActionResult> Mover(int id, int destinoId) { await _service.MoveAsync(id, destinoId); return NoContent(); }

    [HttpGet("emcurso/{utilizadorId:int}")]
    public async Task<IEnumerable<TarefaDto>> EmCurso(int utilizadorId) => await _service.ListarEmCursoAsync(utilizadorId);

    [HttpGet("concluidas/{utilizadorId:int}")]
    public async Task<IEnumerable<TarefaDto>> Concluidas(int utilizadorId, DateTime inicio, DateTime fim) =>
        await _service.ListarConcluidasAsync(utilizadorId, inicio, fim);

    [HttpGet("relatorio/{utilizadorId:int}/{ano:int}/{mes:int}")]
    public async Task<RelatorioMensalDto> Relatorio(int utilizadorId, int ano, int mes) =>
        await _service.RelatorioMensalAsync(utilizadorId, ano, mes);
    
    [HttpGet("projeto/{projetoId:int}")]
    public async Task<IEnumerable<TarefaDto>> PorProjeto(int projetoId) =>
        await _service.GetByProjetoIdAsync(projetoId);

    [HttpGet("utilizador/{utilizadorId:int}")]
    public async Task<IEnumerable<TarefaDto>> PorUtilizador(int utilizadorId)
        => await _service.GetByUtilizadorIdAsync(utilizadorId);

}