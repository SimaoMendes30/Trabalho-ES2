using Backend.AutoMapperProfiles;
using Backend.Models;
using Backend.Repositories;
using Backend.Repositories.Interfaces;
using Backend.Services;
using Backend.Services.Interfaces;
using Backend.Domain.Strategies; // IPrecoTarefaStrategy & DefaultPrecoStrategy
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// JWT Authentication
// ─────────────────────────────────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwt["Issuer"],
            ValidAudience            = jwt["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// DbContext
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContextFactory<sgscDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────────────────────────────────────
// Repositories (DI)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
builder.Services.AddScoped<IProjetoRepository   , ProjetoRepository>();
builder.Services.AddScoped<IMembroRepository    , MembroRepository>();
builder.Services.AddScoped<ITarefaRepository    , TarefaRepository>();

// ─────────────────────────────────────────────────────────────────────────────
// Domain Strategies / Helpers
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IPrecoTarefaStrategy, DefaultPrecoStrategy>();

// ─────────────────────────────────────────────────────────────────────────────
// Services (DI)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUtilizadorService, UtilizadorService>();
builder.Services.AddScoped<IProjetoService   , ProjetoService>();
builder.Services.AddScoped<IMembroService    , MembroService>();
builder.Services.AddScoped<ITarefaService    , TarefaService>();

// ─────────────────────────────────────────────────────────────────────────────
// AutoMapper & Controllers
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

// ─────────────────────────────────────────────────────────────────────────────
// CORS - registrar antes do Build
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .WithOrigins("http://localhost:5267") // ou "https://localhost:5267" se o SPA usar HTTPS
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// ─────────────────────────────────────────────────────────────────────────────
// Swagger
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ─────────────────────────────────────────────────────────────────────────────
// Logging
// ─────────────────────────────────────────────────────────────────────────────
builder.Logging.AddConsole();

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// Middleware pipeline
// ─────────────────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", ctx =>
    {
        ctx.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
