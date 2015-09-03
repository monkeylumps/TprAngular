using System.Data.Entity;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Commands.Handlers
{
    public class UpdateBoardColumnCommandHandler  : ICommandHandler<UpdateBoardColumnCommand, BoardColumn>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public UpdateBoardColumnCommandHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardColumn> HandleAsync(UpdateBoardColumnCommand command)
        {
            if (!await dataContext.Set<BoardEntity>().AnyAsync(x => x.Slug == command.BoardSlug))
            {
                throw new BoardNotFoundException();
            }

            var boardColumnEntity = await dataContext.Set<BoardColumnEntity>()
                .FirstOrDefaultAsync(x => x.Slug == command.BoardColumnSlug && x.BoardEntity.Slug == command.BoardSlug);

            if (boardColumnEntity == null)
            {
                throw new BoardColumnNotFoundException();
            }

            mappingService.Map(command.BoardColumn, boardColumnEntity);


            dataContext.SetModified(boardColumnEntity);
            await dataContext.SaveChangesAsync();

            return mappingService.Map<BoardColumn>(boardColumnEntity);
        }
    }
}