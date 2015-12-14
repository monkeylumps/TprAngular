using System.Web.Http;
using Mindscape.Raygun4Net.WebApi;

namespace KanbanBoardApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RaygunWebApiClient.Attach(config);
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );
        }
    }
}