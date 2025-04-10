using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTO_s;
using AutoMapper;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly SgscContext _context;
        private readonly IMapper _mapper;

        public ProjetoController(SgscContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ProjetoDTO>> PostProjeto(
            [FromBody] ProjetoDTO projetoDTO,
            [FromHeader(Name = "X-User-Id")] int userId)
        {
            try
            {
                Console.WriteLine($">>> RECEBIDO POST /api/projeto para utilizador: {userId} | Projeto: {projetoDTO.Nome}");

                // Verifica se o utilizador existe
                var utilizador = await _context.Utilizadors.FindAsync(userId);
                if (utilizador == null)
                    return BadRequest("Utilizador não encontrado.");

                // Protege contra duplicados mesmo em caso de retry
                var projetoExistente = await _context.Projetos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Nome == projetoDTO.Nome && p.IdUtilizador == userId);

                if (projetoExistente != null)
                    return Conflict("Já existe um projeto com este nome para este utilizador.");

                // Associa o utilizador
                projetoDTO.IdUtilizador = userId;

                // Mapear e guardar
                var projeto = _mapper.Map<Projeto>(projetoDTO);
                _context.Projetos.Add(projeto);
                await _context.SaveChangesAsync();

                projetoDTO.IdProjeto = projeto.IdProjeto;

                return CreatedAtAction(nameof(GetProjeto), new { id = projeto.IdProjeto }, projetoDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno ao criar projeto.",
                    detalhes = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjetoDTO>> GetProjeto(int id)
        {
            var projeto = await _context.Projetos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProjeto == id);

            if (projeto == null)
                return NotFound("Projeto não encontrado.");

            var projetoDTO = new ProjetoDTO
            {
                IdProjeto = projeto.IdProjeto,
                Nome = projeto.Nome,
                NomeCliente = projeto.NomeCliente,
                Descricao = projeto.Descricao,
                PrecoHora = projeto.PrecoHora,
                IdUtilizador = projeto.IdUtilizador,
                DataCriacao = projeto.DataCriacao
            };

            return Ok(projetoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjeto(int id, ProjetoDTO projetoDTO)
        {
            if (id != projetoDTO.IdProjeto)
                return BadRequest("ID do projeto não coincide.");

            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null)
                return NotFound("Projeto não encontrado.");

            _mapper.Map(projetoDTO, projeto);
            _context.Entry(projeto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjetoExists(id))
                    return NotFound("Projeto não encontrado após tentativa de atualização.");

                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjeto(int id)
        {
            var projeto = await _context.Projetos
                .Include(p => p.Membros)
                .Include(p => p.IdTarefas)
                .FirstOrDefaultAsync(p => p.IdProjeto == id);

            if (projeto == null)
                return NotFound("Projeto não encontrado.");

            projeto.Membros.Clear();
            projeto.IdTarefas.Clear();

            _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.IdProjeto == id);
        }
    }
}