namespace Backend.Services;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using AutoMapper;
using Backend.DTOs.Membros;
using Backend.Models;
public sealed class MembroService : IMembroService
{
    private readonly IMembroRepository _repo;
    private readonly IMapper _mapper;
    public MembroService(IMembroRepository repo, IMapper mapper)
    { _repo = repo; _mapper = mapper; }

    public async Task<IEnumerable<MembroDto>> GetByProjetoAsync(int projetoId) =>
        _mapper.Map<IEnumerable<MembroDto>>(await _repo.GetByProjetoIdAsync(projetoId));

    public async Task AddAsync(CreateMembroDto dto)
    {
        await _repo.AddAsync(new Membro
        {
            IdProjeto = dto.ProjetoId,
            IdUtilizador = dto.UtilizadorId,
            EstadoConvite = "Aceite",
            DataConvite = DateOnly.FromDateTime(DateTime.UtcNow)
        });
    }

    public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
