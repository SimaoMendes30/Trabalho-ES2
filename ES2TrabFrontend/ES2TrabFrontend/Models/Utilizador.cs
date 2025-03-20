namespace ES2TrabFrontend.Models
{
    public class Utilizador
    {
        public int IdUtilizador { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal NumHoras { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<Membro> Membros { get; set; } = new();
        public List<Projeto> Projetos { get; set; } = new();
        public List<Tarefa> IdTarefas { get; set; } = new();
    }
}