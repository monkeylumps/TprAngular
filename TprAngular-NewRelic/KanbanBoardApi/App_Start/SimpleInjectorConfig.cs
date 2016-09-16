using System.Web.Http;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using KanbanBoardApi.Mapping;
using KanbanBoardApi.Queries.Handlers;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace KanbanBoardApi
{
    public class SimpleInjectorConfig
    {
        public static void Register(Container container)
        {
            container.Register(typeof (IQueryHandler<,>), new[] {typeof (IQueryHandler<,>).Assembly});
            container.Register(typeof (ICommandHandler<,>), new[] {typeof (ICommandHandler<,>).Assembly});

            container.RegisterCollection(typeof (IHyperMediaState), typeof (IHyperMediaState).Assembly);
            container.Register<IBoardState, BoardState>();
            container.Register<IBoardTaskState, BoardTaskState>();
            container.Register<IBoardColumnState, BoardColumnState>();

            container.Register<ISlugService, SlugService>();

            container.Register<ICommandDispatcher, CommandDispatcher>();
            container.Register<IQueryDispatcher, QueryDispatcher>();
            container.Register<ILinkFactory, LinkFactory>();
            container.Register<IHyperMediaFactory, HyperMediaFactory>();
            container.Register<IMappingService, MappingService>();

            container.RegisterWebApiRequest<IDataContext, DataContext>();

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.EnableHttpRequestMessageTracking(GlobalConfiguration.Configuration);
            container.RegisterSingleton<IRequestMessageProvider>(new RequestMessageProvider(container));

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}