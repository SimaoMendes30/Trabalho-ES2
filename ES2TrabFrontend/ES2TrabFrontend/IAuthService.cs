using System.Threading.Tasks;

public interface IAuthService
{
    Task<bool> ValidateUserAsync(string username, string password);
}