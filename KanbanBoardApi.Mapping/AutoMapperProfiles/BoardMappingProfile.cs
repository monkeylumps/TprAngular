using AutoMapper;

namespace KanbanBoardApi.Mapping.AutoMapperProfiles
{
    public class BoardMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Domain.Board, Dto.Board>();
            CreateMap<Dto.Board, Domain.Board>();
        }
    }
}