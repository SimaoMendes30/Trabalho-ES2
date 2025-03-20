using System.Text.Json.Serialization;

namespace ES2Backend.Models;

public partial class Projeto
{
    public int IdProjeto { get; set; }
    public string Nome { get; set; } = null!;
    public string NomeCliente { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal PrecoHora { get; set; }
    public int IdUtilizador { get; set; }

    [JsonIgnore]  // 🚀 Evita ciclo infinito na serialização
    public virtual Utilizador IdUtilizadorNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();

    [JsonIgnore]
    public virtual ICollection<Tarefa> IdTarefas { get; set; } = new List<Tarefa>();
}
