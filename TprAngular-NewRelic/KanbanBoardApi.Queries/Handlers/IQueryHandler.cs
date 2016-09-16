using System.Threading.Tasks;

namespace KanbanBoardApi.Queries.Handlers
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}