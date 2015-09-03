namespace KanbanBoardApi.HyperMedia
{
    public interface ILinkFactory
    {
        string Build(string routeName, object routeValues);
        object GetRouteValue(string key);
    }
}