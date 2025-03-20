namespace ES2TrabFrontend.Models
{
    public class Projeto
    {
        public int IdProjeto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string NomeCliente { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal PrecoHora { get; set; }
        public int IdUtilizador { get; set; }

        public Utilizador IdUtilizadorNavigation { get; set; } = new();
        public List<Membro> Membros { get; set; } = new();
        public List<Tarefa> IdTarefas { get; set; } = new();
    }
}