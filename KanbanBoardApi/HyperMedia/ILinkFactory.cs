using System.Collections.Generic;
using System.ComponentModel;

namespace KanbanBoardApi.HyperMedia
{
    public interface ILinkFactory
    {
        string Build(string routeName, object routeValues);
        object GetRoutevalue(string key);
    }
}