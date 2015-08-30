using System.Threading.Tasks;

namespace KanbanBoardApi.Dispatchers
{
    public interface ICommandDispatcher
    {
        Task<TResult> HandleAsync<TCommand, TResult>(TCommand command);
    }
}