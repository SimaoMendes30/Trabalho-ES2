using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ES2TrabFrontend.Services;
using ES2TrabFrontend;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ⚙️ Configurar HttpClient para o backend, SEM https se não usares https no backend
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5055/") // Usa http, e verifica se a porta está certa
});

// ✅ Registar os serviços corretamente
builder.Services.AddScoped<ProjetoService>();
builder.Services.AddScoped<MembroService>();
builder.Services.AddScoped<UtilizadorService>();
builder.Services.AddScoped<TarefaService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Serviço de autenticação (login/register)

await builder.Build().RunAsync();