using System.Threading.Tasks;
using KanbanBoardApi.Queries;

namespace KanbanBoardApi.Dispatchers
{
    public interface IQueryDispatcher
    {
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }
}