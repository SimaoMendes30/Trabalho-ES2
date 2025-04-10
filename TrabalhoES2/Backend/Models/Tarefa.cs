using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.Models;

public partial class Tarefa
{
    public int IdTarefa { get; set; }

    public string Descricao { get; set; } = null!;
    
    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public string Estado { get; set; } = null!;

    public int Responsavel { get; set; }
    
    public decimal? PrecoHora { get; set; }
    
    public DateTime DataHoraInicio { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual ICollection<Utilizador> Utilizadores { get; set; } = new List<Utilizador>();
}