using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IMembroRepository
{
    Task<IEnumerable<Membro>> GetByProjetoIdAsync(int projetoId);
    Task AddAsync(Membro membro);
    Task DeleteAsync(int id);
}