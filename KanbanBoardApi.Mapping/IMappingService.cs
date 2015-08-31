namespace KanbanBoardApi.Mapping
{
    public interface IMappingService
    {
        TDestination Map<TDestination>(object source);
    }
}