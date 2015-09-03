using System.Data.Entity;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;

namespace KanbanBoardApi.Commands.Handlers
{
    public class DeleteBoardColumnCommandHandler : ICommandHandler<DeleteBoardColumnCommand, string>
    {
        private readonly IDataContext dataContext;

        public DeleteBoardColumnCommandHandler(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<string> HandleAsync(DeleteBoardColumnCommand command)
        {
            if (!await dataContext.Set<BoardEntity>().AnyAsync(x => x.Slug == command.BoardSlug))
            {
                throw new BoardNotFoundException();
            }

            var boardColumnEntity = await dataContext.Set<BoardColumnEntity>().FirstOrDefaultAsync(x => x.Slug == command.BoardColumnSlug && x.BoardEntity.Slug == command.BoardSlug);

            if (boardColumnEntity == null)
            {
                throw new BoardColumnNotFoundException();
            }

            if (
                await
                    dataContext.Set<BoardTaskEntity>()
                        .AnyAsync(
                            x =>
                                x.BoardColumnEntity.Slug == command.BoardColumnSlug &&
                                x.BoardColumnEntity.BoardEntity.Slug == command.BoardSlug))
            {
                throw new BoardColumnNotEmptyException();
            }

            dataContext.Delete(boardColumnEntity);
            await dataContext.SaveChangesAsync();

            return command.BoardColumnSlug;
        }
    }
}