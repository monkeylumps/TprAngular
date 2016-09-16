using System.Threading.Tasks;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Commands.Handlers;
using SimpleInjector;

namespace KanbanBoardApi.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Container container;

        public CommandDispatcher(Container container)
        {
            this.container = container;
        }

        public Task<TResult> HandleAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = container.GetInstance<ICommandHandler<TCommand, TResult>>();
            return commandHandler.HandleAsync(command);
        }
    }
}