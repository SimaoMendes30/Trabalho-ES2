namespace Backend.DTO_s
{
    public class TarefaInicioDTO
    {
        public string Descricao { get; set; } = null!;
        public DateTime DataHoraInicio { get; set; }
        public int Responsavel { get; set; }
        public int IdProjeto { get; set; } // 🆕 novo campo
    }
}