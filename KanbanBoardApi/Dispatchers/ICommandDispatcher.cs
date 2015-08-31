using System.Threading.Tasks;
using KanbanBoardApi.Commands;

namespace KanbanBoardApi.Dispatchers
{
    public interface ICommandDispatcher
    {
        Task<TResult> HandleAsync<TCommand, TResult>(TCommand command) where TCommand :ICommand;
    }
}