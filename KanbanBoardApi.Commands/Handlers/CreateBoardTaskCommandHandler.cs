using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Commands.Handlers
{
    public class CreateBoardTaskCommandHandler : ICommandHandler<CreateBoardTaskCommand, BoardTask>
    {
        private IDataContext dataContext;
        private IMappingService mappingService;

        public CreateBoardTaskCommandHandler(IDataContext dataContext, IMappingService  mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardTask> HandleAsync(CreateBoardTaskCommand command)
        {
            var boardTask = mappingService.Map<BoardTaskEntity>(command.BoardTask);

            if (!await dataContext.Set<BoardEntity>().AnyAsync(x => x.Slug == command.BoardSlug))
            {
                throw new BoardNotFoundException();
            }

            var boardColumn =
                await dataContext.Set<BoardEntity>()
                    .Where(x => x.Slug == command.BoardSlug)
                    .Select(x => x.Columns.FirstOrDefault(y => y.Slug == command.BoardTask.BoardColumnSlug))
                    .FirstOrDefaultAsync();

            if (boardColumn == null)
            {
                throw new BoardColumnNotFoundException();
            }

            boardTask.BoardColumnEntity = boardColumn;
            dataContext.Set<BoardTaskEntity>().Add(boardTask);

            await dataContext.SaveChangesAsync();
            return mappingService.Map<BoardTask>(boardTask);
        }
    }
}