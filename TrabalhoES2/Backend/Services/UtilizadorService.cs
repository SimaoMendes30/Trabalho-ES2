// UtilizadorService.cs
using AutoMapper;
using Backend.DTOs.Utilizadores;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services
{
    public sealed class UtilizadorService : IUtilizadorService
    {
        private readonly IUtilizadorRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<UtilizadorService> _logger;

        public UtilizadorService(
            IConfiguration config,
            IUtilizadorRepository repo,
            IMapper mapper,
            ILogger<UtilizadorService> logger)
        {
            _config = config;
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UtilizadorTokenDto> GerarTokenAsync(UtilizadorLoginDto loginDto)
        {
            // Validar credenciais
            var user = await _repo.GetByUsernameAsync(loginDto.Username)
                       ?? throw new UnauthorizedAccessException("Credenciais inválidas");
            if (user.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Credenciais inválidas");

            // Construir JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUtilizador.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpireMinutes"]!)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Retornar token
            return new UtilizadorTokenDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<UtilizadorDto> CreateAsync(UtilizadorCreateDto dto)
        {
            var entity = _mapper.Map<Utilizador>(dto);
            await _repo.AddAsync(entity);
            return _mapper.Map<UtilizadorDto>(entity);
        }

        public async Task<IEnumerable<UtilizadorDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<UtilizadorDto>>(list);
        }

        public async Task<UtilizadorDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException(
                             $"Utilizador {id} não encontrado");
            return _mapper.Map<UtilizadorDto>(entity);
        }

        public async Task<UtilizadorDetailsDto> GetDetailsAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException(
                             $"Utilizador {id} não encontrado");

            // Montar detalhes: exemplo, adaptar conforme domínio
            var details = new UtilizadorDetailsDto
            {
                NumHoras = null
            };
            // TODO: preencher Membros, Projetos e IdTarefas
            return details;
        }

        public async Task UpdateAsync(int id, UtilizadorUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException(
                             $"Utilizador {id} não encontrado");
            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task UpdatePasswordAsync(int id, UtilizadorUpdatePasswordDto dto)
        {
            var entity = await _repo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException(
                             $"Utilizador {id} não encontrado");
            if (entity.Password != dto.OldPassword)
                throw new UnauthorizedAccessException("Senha antiga inválida");
            entity.Password = dto.NewPassword;
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
