using System.Web.Http.Routing;

namespace KanbanBoardApi.HyperMedia
{
    public class LinkFactory : ILinkFactory
    {
        private readonly UrlHelper urlHelper;

        public LinkFactory(UrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        public string Build(string routeName, object routeValues)
        {
            return urlHelper.Link(routeName, routeValues);
        }
    }
}