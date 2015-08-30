using System.Threading.Tasks;

namespace KanbanBoardApi.Dispatchers
{
    public interface IQueryDispatcher
    {
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query);
    }
}