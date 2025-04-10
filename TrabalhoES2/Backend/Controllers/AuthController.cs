using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTO_s;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SgscContext _context;

        // Construtor com injeção de dependência do DbContext
        public AuthController(SgscContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDTO login)
        {
            var utilizador = _context.Utilizadors.FirstOrDefault(u => u.Username == login.Username);
            
            if (utilizador == null || utilizador.Password != login.Password)
            {
                return Unauthorized("Credenciais inválidas");
            }

            // Retornar o ID do utilizador (e outros dados se necessário)
            return Ok(new { UserId = utilizador.IdUtilizador, Nome = utilizador.Nome });
        }
    }
}