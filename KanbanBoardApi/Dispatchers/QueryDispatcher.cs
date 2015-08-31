using System.Threading.Tasks;
using KanbanBoardApi.Queries;
using KanbanBoardApi.Queries.Handlers;
using SimpleInjector;

namespace KanbanBoardApi.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly Container container;
        public QueryDispatcher(Container container)
        {
            this.container = container;
        }

        public Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            var queryHandler = container.GetInstance<IQueryHandler<TQuery, TResult>>();

            return queryHandler.HandleAsync(query);
        }
    }
}