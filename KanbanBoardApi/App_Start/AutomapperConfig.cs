using AutoMapper;
using KanbanBoardApi.Mapping.AutoMapperProfiles;

namespace KanbanBoardApi
{
    public class AutomapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(
               x =>
               {
                   x.AddProfile<BoardMappingProfile>();
               });
        } 
    }
}