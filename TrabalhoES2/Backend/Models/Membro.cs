using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.Models;

public partial class Membro
{
    public int IdMembro { get; set; }

    public int IdUtilizador { get; set; }

    public int IdProjeto { get; set; }

    public DateOnly? DataConvite { get; set; }

    public DateOnly? DataEstado { get; set; }

    public string EstadoConvite { get; set; } = null!;

    public string? EstadoAtividade { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Projeto Projeto { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Utilizador Utilizador { get; set; } = null!;
}