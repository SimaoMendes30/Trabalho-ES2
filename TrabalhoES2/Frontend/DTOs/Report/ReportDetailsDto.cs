namespace Frontend.DTOs.Report;

public class ReportDetailsDto
{
    public DateOnly Day { get; set; }
    public string Projeto { get; set; }
    public string TituloTarefa { get; set; }
    public string Cliente { get; set; }
    public string Utilizadores { get; set; }
    public double Horas { get; set; }
    public decimal Custo { get; set; }
}