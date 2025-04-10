using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método para fazer login
        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }

        // Método para fazer logout
        public void Logout()
        {
            // Limpar tokens ou informações de sessão
        }

        // Método para verificar se o usuário está autenticado
        public bool IsAuthenticated()
        {
            // Aqui você pode verificar o token JWT ou qualquer outra forma de autenticação
            return true; // Exemplo simples
        }
    }
}