namespace Backend.Services;

using AutoMapper;
using Repositories.Interfaces;
using Services.Interfaces;
using DTOs.Tarefas;
using DTOs.Relatorios;
using Domain.Builders;
using Domain.Strategies;
using Domain.Factories;
public sealed class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _repo;
    private readonly IProjetoRepository _projetoRepo;
    private readonly IMapper _mapper;
    private readonly IPrecoTarefaStrategy _precoStrategy;
    private readonly ILogger<TarefaService> _logger;

    public TarefaService(ITarefaRepository repo, IProjetoRepository projetoRepo, IMapper mapper,
                         IPrecoTarefaStrategy precoStrategy, ILogger<TarefaService> logger)
    {
        _repo = repo; _projetoRepo = projetoRepo; _mapper = mapper;
        _precoStrategy = precoStrategy; _logger = logger;
    }

    public async Task<TarefaDto> StartAsync(StartTarefaDto dto)
    {
        var projeto = await _projetoRepo.GetByIdAsync(dto.ProjetoId);
        var tarefa = TarefaFactory.Criar(dto.Descricao, dto.Responsavel, dto.PrecoHora);
        tarefa.IdProjetos.Add(projeto);
        await _repo.AddAsync(tarefa);
        return _mapper.Map<TarefaDto>(tarefa);
    }

    public async Task<TarefaDto> EndAsync(int id, EndTarefaDto dto)
    {
        var tarefa = await _repo.GetByIdAsync(id);

        if (tarefa.DataFim != null)
            throw new InvalidOperationException("Tarefa já concluída.");

        // Conversão explícita de DateTime? para DateOnly?
        DateOnly dataFim = dto.DataFim.HasValue
            ? DateOnly.FromDateTime(dto.DataFim.Value)
            : DateOnly.FromDateTime(DateTime.UtcNow);

        tarefa.DataFim = dataFim;
        tarefa.Estado = "Concluída";

        await _repo.UpdateAsync(tarefa);

        return _mapper.Map<TarefaDto>(tarefa);
    }



    public async Task MoveAsync(int tarefaId, int projetoDestinoId)
    {
        var tarefa = await _repo.GetByIdAsync(tarefaId);
        var destino = await _projetoRepo.GetByIdAsync(projetoDestinoId);
        tarefa.IdProjetos.Clear();
        tarefa.IdProjetos.Add(destino);
        await _repo.UpdateAsync(tarefa);
    }

    public async Task<IEnumerable<TarefaDto>> ListarEmCursoAsync(int utilizadorId)
    {
        var tarefas = await _repo.GetEmCursoAsync(utilizadorId);
        return _mapper.Map<IEnumerable<TarefaDto>>(tarefas);
    }

    public async Task<IEnumerable<TarefaDto>> ListarConcluidasAsync(int utilizadorId, DateTime inicio, DateTime fim)
    {
        var tarefas = await _repo.GetConcluidasEntreDatasAsync(utilizadorId, inicio, fim);
        return _mapper.Map<IEnumerable<TarefaDto>>(tarefas);
    }

    public async Task<RelatorioMensalDto> RelatorioMensalAsync(int utilizadorId, int ano, int mes)
    {
        var inicio = new DateTime(ano, mes, 1);
        var fim = inicio.AddMonths(1).AddSeconds(-1);
        var concluidas = await _repo.GetConcluidasEntreDatasAsync(utilizadorId, inicio, fim);
        return new RelatorioMensalBuilder().Add(concluidas).Build();
    }
    
    public async Task<IEnumerable<TarefaDto>> GetByProjetoIdAsync(int projetoId)
    {
        var tarefas = await _repo.GetByProjetoIdAsync(projetoId);
        return _mapper.Map<IEnumerable<TarefaDto>>(tarefas);
    }
    
    public async Task<IEnumerable<TarefaDto>> GetByUtilizadorIdAsync(int utilizadorId)
    {
        var tarefas = await _repo.GetByUtilizadorIdAsync(utilizadorId);
        return _mapper.Map<IEnumerable<TarefaDto>>(tarefas);
    }
}
