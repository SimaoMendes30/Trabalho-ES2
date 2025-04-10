namespace Frontend.DTO_s
{
    public class TarefaDTO
    {
        public int IdTarefa { get; set; }
        public string Descricao { get; set; } = null!;
        public DateOnly DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
        public string Estado { get; set; } = null!;
        public int Responsavel { get; set; }
    }
}