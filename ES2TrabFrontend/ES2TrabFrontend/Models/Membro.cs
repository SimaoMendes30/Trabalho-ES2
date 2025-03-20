namespace ES2TrabFrontend.Models
{
    public class Membro
    {
        public int IdMembro { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public int IdUtilizador { get; set; }
        public int IdProjeto { get; set; }

        public Projeto IdProjetoNavigation { get; set; } = new();
        public Utilizador IdUtilizadorNavigation { get; set; } = new();
    }
}