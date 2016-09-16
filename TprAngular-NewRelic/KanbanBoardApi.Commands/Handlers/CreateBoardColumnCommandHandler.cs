using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Commands.Handlers
{
    public class CreateBoardColumnCommandHandler : ICommandHandler<CreateBoardColumnCommand, BoardColumn>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;
        private readonly ISlugService slugService;

        public CreateBoardColumnCommandHandler(IDataContext dataContext, IMappingService mappingService,
            ISlugService slugService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
            this.slugService = slugService;
        }

        public async Task<BoardColumn> HandleAsync(CreateBoardColumnCommand command)
        {
            var boardColumn = mappingService.Map<BoardColumnEntity>(command.BoardColumn);
            boardColumn.Slug = slugService.Slugify(boardColumn.Name);

            if (dataContext.Set<BoardColumnEntity>().Any(x => x.Slug == boardColumn.Slug))
            {
                throw new CreateBoardColumnCommandSlugExistsException();
            }

            var board = dataContext.Set<BoardEntity>().FirstOrDefault(x => x.Slug == command.BoardSlug);
            if (board == null)
            {
                throw new BoardNotFoundException();
            }

            dataContext.Set<BoardColumnEntity>().Add(boardColumn);
            board.Columns.Add(boardColumn);

            await dataContext.SaveChangesAsync();
            return mappingService.Map<BoardColumn>(boardColumn);
        }
    }
}