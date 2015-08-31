using AutoMapper;

namespace KanbanBoardApi.Mapping
{
    public class MappingService : IMappingService
    {
        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}