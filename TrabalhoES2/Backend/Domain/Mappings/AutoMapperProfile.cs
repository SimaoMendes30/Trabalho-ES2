using AutoMapper;
using Backend.Domain.DTOs.Member;
using Backend.Domain.DTOs.Project;
using Backend.Domain.DTOs.Task;
using Backend.Domain.DTOs.User;
using Backend.Models;

namespace Backend.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Member
            CreateMap<MemberEntity, MemberCreateDto>().ReverseMap();
            CreateMap<MemberEntity, MemberDetailsDto>().ReverseMap();
            CreateMap<MemberEntity, MemberDetailsExtendedDto>()
                .IncludeBase<MemberEntity, MemberDetailsDto>()                        // reaproveita o mapeamento básico
                .ForMember(dest => dest.IdUserNavigation,                             // popula a navegação para o utilizador
                    opt  => opt.MapFrom(src => src.IdUserEntityNavigation))
                .ReverseMap();

            CreateMap<MemberEntity, MemberUpdateDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Project
            CreateMap<ProjectEntity, ProjectCreateDto>().ReverseMap();

            CreateMap<ProjectEntity, ProjectDetailsDto>()
                .ReverseMap()
                .ForMember(dest => dest.Responsavel, opt => opt.Ignore()) // ← proteção do responsável
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ProjectEntity, ProjectDetailsExtendedDto>()
                .IncludeBase<ProjectEntity, ProjectDetailsDto>()                      // reaproveita o mapeamento de ProjectDetailsDto
                .ForMember(dest => dest.ResponsavelNavigation,                        // popula o responsável
                    opt  => opt.MapFrom(src => src.ResponsavelNavigation))
                .ForMember(dest => dest.Membros,                                      // mapeia a lista de membros
                    opt  => opt.MapFrom(src => src.Membros));

            CreateMap<ProjectEntity, ProjectUpdateDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            // Task
            CreateMap<TaskEntity, TaskCreateDto>().ReverseMap();
            CreateMap<TaskEntity, TaskDetailsDto>().ReverseMap();
            CreateMap<TaskEntity, TaskDetailsExtendedDto>().ReverseMap();
            CreateMap<TaskEntity, TaskUpdateDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // User
            CreateMap<UserEntity, UserCreateDto>().ReverseMap();
            CreateMap<UserEntity, UserDetailsDto>().ReverseMap();
            CreateMap<UserEntity, UserDetailsExtendedDto>().ReverseMap();
            CreateMap<UserEntity, UserUpdateDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}