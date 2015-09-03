using System.Data.Entity;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;

namespace KanbanBoardApi.Commands.Handlers
{
    public class DeleteBoardTaskCommandHandler : ICommandHandler<DeleteBoardTaskCommand, int>
    {
        private readonly IDataContext dataContext;

        public DeleteBoardTaskCommandHandler(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<int> HandleAsync(DeleteBoardTaskCommand command)
        {
            if (!await dataContext.Set<BoardEntity>().AnyAsync(x => x.Slug == command.BoardSlug))
            {
                throw new BoardNotFoundException();
            }

            var boardTaskEntity = await dataContext.Set<BoardTaskEntity>().FirstOrDefaultAsync(x => x.Id == command.BoardTaskId);

            if (boardTaskEntity == null)
            {
                throw new BoardTaskNotFoundException();
            }

            dataContext.Delete(boardTaskEntity);
            await dataContext.SaveChangesAsync();

            return command.BoardTaskId;
        }
    }
}