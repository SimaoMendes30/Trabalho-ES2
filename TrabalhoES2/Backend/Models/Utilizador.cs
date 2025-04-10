using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.Models;

public partial class Utilizador
{
    public int IdUtilizador { get; set; }

    public string Nome { get; set; } = null!;

    public int? NumHoras { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Admin { get; set; }

    public bool SuperUser { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
}
