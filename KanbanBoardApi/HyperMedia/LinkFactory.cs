using System.Web.Http.Routing;

namespace KanbanBoardApi.HyperMedia
{
    public class LinkFactory : ILinkFactory
    {
        private readonly IRequestMessageProvider requestMessageProvider;
        private readonly UrlHelper urlHelper;

        public LinkFactory(IRequestMessageProvider requestMessageProvider)
        {
            this.requestMessageProvider = requestMessageProvider;
            this.urlHelper = new UrlHelper();
        }

        public string Build(string routeName, object routeValues)
        {
            urlHelper.Request = requestMessageProvider.CurrentMessage;
            return urlHelper.Link(routeName, routeValues);
        }
    }
}