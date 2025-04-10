using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTO_s;
using AutoMapper;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadorController : ControllerBase
    {
        private readonly SgscContext _context;
        private readonly IMapper _mapper;

        public UtilizadorController(SgscContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST: api/Utilizador (Criar conta)
        [HttpPost]
        public async Task<ActionResult<UtilizadorDTO>> PostUtilizador(UtilizadorDTO utilizadorDTO)
        {
            var existingUser = await _context.Utilizadors
                .FirstOrDefaultAsync(u => u.Username == utilizadorDTO.Username);

            if (existingUser != null)
            {
                return Conflict("Já existe um utilizador com este nome de utilizador.");
            }

            var utilizador = _mapper.Map<Utilizador>(utilizadorDTO);

            _context.Utilizadors.Add(utilizador);
            await _context.SaveChangesAsync();

            var createdUtilizadorDTO = _mapper.Map<UtilizadorDTO>(utilizador);
            return CreatedAtAction(nameof(GetUtilizador), new { id = utilizador.IdUtilizador }, createdUtilizadorDTO);
        }

        // GET: api/Utilizador/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UtilizadorDTO>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizadors
                .Include(u => u.Projetos)
                .Include(u => u.Tarefas)
                .FirstOrDefaultAsync(u => u.IdUtilizador == id);

            if (utilizador == null)
            {
                return NotFound();
            }

            var utilizadorDTO = _mapper.Map<UtilizadorDTO>(utilizador);
            return Ok(utilizadorDTO);
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarUtilizador(int id, [FromBody] UtilizadorUpdateDTO dadosAtualizados)
        {
            var utilizador = await _context.Utilizadors.FindAsync(id);
            if (utilizador == null)
                return NotFound();

            if (utilizador.Username != dadosAtualizados.Username)
            {
                var usernameExistente = await _context.Utilizadors
                    .AnyAsync(u => u.Username == dadosAtualizados.Username && u.IdUtilizador != id);

                if (usernameExistente)
                    return Conflict("Este nome de utilizador já está em uso.");
            }

            utilizador.Nome = dadosAtualizados.Nome;
            utilizador.NumHoras = dadosAtualizados.NumHoras;
            utilizador.Username = dadosAtualizados.Username;
            utilizador.Password = dadosAtualizados.Password;

            await _context.SaveChangesAsync();

            return Ok("Dados do utilizador atualizados com sucesso.");
        }

    }
    
}