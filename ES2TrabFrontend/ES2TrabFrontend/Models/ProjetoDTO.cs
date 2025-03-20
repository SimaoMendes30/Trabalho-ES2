namespace ES2TrabFrontend.Models
{
    public class ProjetoDTO
    {
        public int IdProjeto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string NomeCliente { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal PrecoHora { get; set; }
        public int IdUtilizador { get; set; }
    }
}