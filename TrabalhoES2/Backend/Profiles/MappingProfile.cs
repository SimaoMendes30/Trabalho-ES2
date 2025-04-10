using AutoMapper;
using Backend.Models;
using Backend.DTO_s;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Tarefa
        CreateMap<Tarefa, TarefaDTO>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado));

        // Projeto
        CreateMap<Projeto, ProjetoDTO>();
        CreateMap<ProjetoDTO, Projeto>()
            .ForMember(dest => dest.IdProjeto, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore());

        // Membro
        CreateMap<Membro, MembroDTO>();
        CreateMap<MembroDTO, Membro>()
            .ForMember(dest => dest.IdMembro, opt => opt.Ignore());

        // Utilizador → UtilizadorDTO
        CreateMap<Utilizador, UtilizadorDTO>()
            .ForMember(dest => dest.IdProjetos, opt => opt.MapFrom(src => src.Projetos.Select(p => p.IdProjeto)))
            .ForMember(dest => dest.IdTarefas, opt => opt.MapFrom(src => src.Tarefas.Select(t => t.IdTarefa)));

        // UtilizadorDTO → Utilizador
        CreateMap<UtilizadorDTO, Utilizador>()
            .ForMember(dest => dest.IdUtilizador, opt => opt.Ignore());
    }
}