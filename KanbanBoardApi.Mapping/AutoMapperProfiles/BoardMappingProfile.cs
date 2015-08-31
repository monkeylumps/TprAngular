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

            CreateMap<BoardTaskEntity, BoardTask>();
            CreateMap<BoardTask, BoardTaskEntity>();
        }
    }
}