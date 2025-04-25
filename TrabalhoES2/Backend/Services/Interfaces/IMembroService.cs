namespace Backend.Services.Interfaces;
using Backend.DTOs.Membros;
public interface IMembroService
{
    Task<IEnumerable<MembroDto>> GetByProjetoAsync(int projetoId);
    Task AddAsync(CreateMembroDto dto);
    Task DeleteAsync(int id);
}