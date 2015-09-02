using AutoMapper;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.Mapping.AutoMapperProfiles
{
    public class BoardMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BoardEntity, Board>();
            CreateMap<Board, BoardEntity>();

            CreateMap<BoardColumnEntity, BoardColumn>();
            CreateMap<BoardColumn, BoardColumnEntity>();

            CreateMap<BoardTaskEntity, BoardTask>()
                .ForMember(x => x.BoardColumnSlug, x => x.MapFrom(y => y.BoardColumnEntity.Slug));

            CreateMap<BoardTask, BoardTaskEntity>();
        }
    }
}