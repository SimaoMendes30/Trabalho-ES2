namespace Frontend.DTO_s
{
    public class UtilizadorDTO
    {
        public int IdUtilizador { get; set; }

        public string Nome { get; set; } = null!;

        public int? NumHoras { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!; // ✅ ADICIONAR ISTO

        public bool Admin { get; set; }

        public bool SuperUser { get; set; }

        public ICollection<int> IdProjetos { get; set; } = new List<int>();

        public ICollection<int> IdTarefas { get; set; } = new List<int>();
    }
}