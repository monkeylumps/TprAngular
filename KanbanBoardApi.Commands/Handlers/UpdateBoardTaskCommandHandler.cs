using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Commands.Handlers
{
    public class UpdateBoardTaskCommandHandler : ICommandHandler<UpdateBoardTaskCommand, BoardTask>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public UpdateBoardTaskCommandHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardTask> HandleAsync(UpdateBoardTaskCommand command)
        {
            var boardTaskEntity = await dataContext.Set<BoardTaskEntity>().FirstOrDefaultAsync(x => x.Id == command.BoardTask.Id);

            if (boardTaskEntity == null)
            {
                throw new BoardTaskNotFoundException();
            }

            var boardColumnEntity = await dataContext.Set<BoardColumnEntity>()
                .Where(x => x.Slug == command.BoardTask.BoardColumnSlug && x.BoardEntity.Slug == command.BoardSlug)
                .FirstOrDefaultAsync();

            if (boardColumnEntity == null)
            {
                throw new BoardColumnNotFoundException();
            }

            mappingService.Map(command.BoardTask, boardTaskEntity);

            boardTaskEntity.BoardColumnEntity = boardColumnEntity;

            dataContext.SetModified(boardTaskEntity);

            await dataContext.SaveChangesAsync();

            return mappingService.Map<BoardTask>(boardTaskEntity);
        }
    }
}