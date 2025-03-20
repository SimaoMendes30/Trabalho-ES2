using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class ProjetosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjetosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjetoDTO>>> GetProjetos()
    {
        var projetos = await _context.Projetos
            .Select(p => new ProjetoDTO
            {
                IdProjeto = p.IdProjeto,
                Nome = p.Nome,
                NomeCliente = p.NomeCliente,
                Descricao = p.Descricao,
                PrecoHora = p.PrecoHora,
                IdUtilizador = p.IdUtilizador
            })
            .ToListAsync();

        return Ok(projetos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjetoDTO>> GetProjeto(int id)
    {
        var projeto = await _context.Projetos
            .Where(p => p.IdProjeto == id)
            .Select(p => new ProjetoDTO
            {
                IdProjeto = p.IdProjeto,
                Nome = p.Nome,
                NomeCliente = p.NomeCliente,
                Descricao = p.Descricao,
                PrecoHora = p.PrecoHora,
                IdUtilizador = p.IdUtilizador
            })
            .FirstOrDefaultAsync();

        if (projeto == null)
        {
            return NotFound();
        }

        return Ok(projeto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjetoDTO>> PostProjeto(ProjetoDTO projetoDTO)
    {
        var projeto = new Projeto
        {
            Nome = projetoDTO.Nome,
            NomeCliente = projetoDTO.NomeCliente,
            Descricao = projetoDTO.Descricao,
            PrecoHora = projetoDTO.PrecoHora,
            IdUtilizador = projetoDTO.IdUtilizador
        };

        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjeto), new { id = projeto.IdProjeto }, projetoDTO);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProjeto(int id, ProjetoDTO projetoDTO)
    {
        if (id != projetoDTO.IdProjeto)
        {
            return BadRequest();
        }

        var projeto = await _context.Projetos.FindAsync(id);
        if (projeto == null)
        {
            return NotFound();
        }

        projeto.Nome = projetoDTO.Nome;
        projeto.NomeCliente = projetoDTO.NomeCliente;
        projeto.Descricao = projetoDTO.Descricao;
        projeto.PrecoHora = projetoDTO.PrecoHora;
        projeto.IdUtilizador = projetoDTO.IdUtilizador;

        _context.Entry(projeto).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjeto(int id)
    {
        var projeto = await _context.Projetos.FindAsync(id);
        if (projeto == null)
        {
            return NotFound();
        }

        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
