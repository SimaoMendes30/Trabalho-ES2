using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTO_s;
using AutoMapper;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembroController : ControllerBase
    {
        private readonly SgscContext _context;
        private readonly IMapper _mapper;

        public MembroController(SgscContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Membro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembroDTO>>> GetMembros()
        {
            var membros = await _context.Membros
                .Include(m => m.Projeto)
                .Include(m => m.Utilizador)
                .ToListAsync();

            var membrosDTO = _mapper.Map<IEnumerable<MembroDTO>>(membros);  // Mapeia a lista de membros para DTOs

            return Ok(membrosDTO);
        }

        // GET: api/Membro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MembroDTO>> GetMembro(int id)
        {
            var membro = await _context.Membros
                .Include(m => m.Projeto)
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.IdMembro == id);

            if (membro == null)
            {
                return NotFound();
            }

            var membroDTO = _mapper.Map<MembroDTO>(membro);  // Mapeia o membro para DTO

            return Ok(membroDTO);
        }

        // POST: api/Membro
        [HttpPost]
        public async Task<ActionResult<MembroDTO>> PostMembro(MembroDTO membroDto)
        {
            var membro = _mapper.Map<Membro>(membroDto);  // Mapeia o DTO para a entidade

            _context.Membros.Add(membro);
            await _context.SaveChangesAsync();

            membroDto.IdMembro = membro.IdMembro;

            return CreatedAtAction(nameof(GetMembro), new { id = membro.IdMembro }, membroDto);
        }

        // PUT: api/Membro/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembro(int id, MembroDTO membroDto)
        {
            if (id != membroDto.IdMembro)
            {
                return BadRequest();
            }

            var membro = await _context.Membros.FindAsync(id);
            if (membro == null)
            {
                return NotFound();
            }

            _mapper.Map(membroDto, membro);  // Mapeia o DTO para a entidade para atualização

            _context.Entry(membro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Membro/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembro(int id)
        {
            var membro = await _context.Membros.FindAsync(id);
            if (membro == null)
            {
                return NotFound();
            }

            _context.Membros.Remove(membro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MembroExists(int id)
        {
            return _context.Membros.Any(e => e.IdMembro == id);
        }
    }
}
