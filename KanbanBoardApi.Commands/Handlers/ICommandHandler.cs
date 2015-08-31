using System.Threading.Tasks;

namespace KanbanBoardApi.Commands.Handlers
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}