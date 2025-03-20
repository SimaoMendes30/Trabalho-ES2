namespace ES2Backend.Models;

public class ProjetoDTO
{
    public int IdProjeto { get; set; }
    public string Nome { get; set; }
    public string NomeCliente { get; set; }
    public string Descricao { get; set; }
    public decimal PrecoHora { get; set; }
    public int IdUtilizador { get; set; } // Sem navegação para IdUtilizadorNavigation
}
