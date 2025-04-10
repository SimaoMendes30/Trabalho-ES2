namespace Frontend.DTO_s
{
    public class MembroDTO
    {
        public int IdMembro { get; set; }
        public int IdUtilizador { get; set; }
        public int IdProjeto { get; set; }
        public DateOnly? DataConvite { get; set; }
        public DateOnly? DataEstado { get; set; }
        public string EstadoConvite { get; set; } = null!;
        public string? EstadoAtividade { get; set; }
    }
}

