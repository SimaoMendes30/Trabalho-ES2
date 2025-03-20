namespace ES2TrabFrontend.Models
{
    public class Tarefa
    {
        public int IdTarefa { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateOnly DataInicio { get; set; }
        public decimal PrecoHora { get; set; }
        public bool Estado { get; set; }

        public List<Projeto> IdProjetos { get; set; } = new();
        public List<Utilizador> IdUtilizadors { get; set; } = new();
    }
}