using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;
using Frontend.Services;
using MudBlazor.Services; // <-- IMPORTANTE

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuração do HttpClient para a comunicação com o backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5157/") });

// Registra os serviços para Utilizador, Tarefa, Projeto, Membro, etc.
builder.Services.AddScoped<UtilizadorService>();  // Serviço para Utilizadores
builder.Services.AddScoped<TarefaService>();      // Serviço para Tarefas
builder.Services.AddScoped<ProjetoService>();     // Serviço para Projetos
builder.Services.AddScoped<MembroService>();      // Serviço para Membros

builder.Services.AddMudServices(); // <-- ADICIONAR ISTO

await builder.Build().RunAsync();