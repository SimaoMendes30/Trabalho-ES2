using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTO_s;
using AutoMapper;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private readonly SgscContext _context;
        private readonly IMapper _mapper;

        public TarefaController(SgscContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Tarefa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> GetTarefas()
        {
            var tarefas = await _context.Tarefas.ToListAsync();
            var tarefaDTOs = _mapper.Map<List<TarefaDTO>>(tarefas);
            return Ok(tarefaDTOs);  // Retorna as tarefas mapeadas para DTO com status HTTP 200
        }

        // GET: api/Tarefa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaDTO>> GetTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
            {
                return NotFound();  // Retorna 404 se a tarefa não for encontrada
            }

            var tarefaDTO = _mapper.Map<TarefaDTO>(tarefa);
            return Ok(tarefaDTO);  // Retorna a tarefa mapeada para DTO com status HTTP 200
        }

        // POST: api/Tarefa
        [HttpPost]
        public async Task<ActionResult<TarefaDTO>> PostTarefa(TarefaDTO tarefaDTO)
        {
            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            var tarefaRetornada = _mapper.Map<TarefaDTO>(tarefa);
            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.IdTarefa }, tarefaRetornada);  // Retorna 201 com a nova tarefa criada mapeada para DTO
        }

        // PUT: api/Tarefa/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarefa(int id, TarefaDTO tarefaDTO)
        {
            if (id != tarefaDTO.IdTarefa)
            {
                return BadRequest();  // Retorna 400 se o ID não coincidir
            }

            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);
            _context.Entry(tarefa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TarefaExists(id))
                {
                    return NotFound();  // Retorna 404 se a tarefa não existir
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Retorna 204 se a atualização for bem-sucedida
        }

        // DELETE: api/Tarefa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();  // Retorna 404 se a tarefa não for encontrada
            }

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return NoContent();  // Retorna 204 se a tarefa for removida com sucesso
        }
        
        // DELETE: api/Tarefa/em-progresso/{id}
        [HttpDelete("em-progresso/{id}")]
        public async Task<IActionResult> RemoverTarefaEmProgresso(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
                return NotFound("Tarefa não encontrada.");

            if (tarefa.Estado != "Em Progresso")
                return BadRequest("Só é possível remover tarefas em progresso.");

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Tarefa em progresso removida com sucesso.",
                IdTarefaRemovida = id
            });
        }

        // Verifica se a tarefa existe na base de dados
        private bool TarefaExists(int id)
        {
            return _context.Tarefas.Any(e => e.IdTarefa == id);
        }
        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarTarefa([FromBody] TarefaInicioDTO dto)
        {
            try
            {
                var utilizador = await _context.Utilizadors.FindAsync(dto.Responsavel);
                if (utilizador == null)
                    return NotFound("Utilizador não encontrado.");

                var novaTarefa = new Tarefa
                {
                    Descricao = dto.Descricao,
                    Estado = "Em Progresso",
                    DataHoraInicio = DateTime.SpecifyKind(dto.DataHoraInicio, DateTimeKind.Utc),
                    Responsavel = dto.Responsavel,
                    DataInicio = DateOnly.FromDateTime(dto.DataHoraInicio)
                };

                _context.Tarefas.Add(novaTarefa);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Mensagem = "Tarefa iniciada com sucesso.",
                    TarefaId = novaTarefa.IdTarefa
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensagem = "Erro interno ao salvar tarefa.",
                    Erro = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        [HttpPost("{idTarefa}/definir-preco-hora")]
        public async Task<IActionResult> DefinirPrecoHoraTarefa(int idTarefa, [FromQuery] int idProjeto)
        {
            var tarefa = await _context.Tarefas.FindAsync(idTarefa);
            if (tarefa == null)
                return NotFound("Tarefa não encontrada.");

            var projeto = await _context.Projetos.FindAsync(idProjeto);
            if (projeto == null)
                return NotFound("Projeto não encontrado.");

            if (projeto.PrecoHora == null)
                return BadRequest("O projeto não tem um preço hora definido.");

            tarefa.PrecoHora = projeto.PrecoHora;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Mensagem = $"Preço hora da tarefa atualizado para {tarefa.PrecoHora}€ com base no projeto '{projeto.Nome}'"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Erro interno ao atualizar preço hora da tarefa.",
                    Detalhes = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        
        [HttpGet("em-progresso/{idUtilizador}")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> GetTarefasEmProgressoPorUtilizador(int idUtilizador)
        {
            var tarefas = await _context.Tarefas
                .Where(t => t.Responsavel == idUtilizador && t.Estado == "Em Progresso")
                .ToListAsync();

            var tarefaDTOs = _mapper.Map<List<TarefaDTO>>(tarefas);
            return Ok(tarefaDTOs);
        }

        
        }


    }
