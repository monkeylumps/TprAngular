using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Commands.Handlers
{
    public class CreateBoardColumnCommandHandler : ICommandHandler<CreateBoardColumnCommand, Dto.BoardColumn>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;
        private readonly ISlugService slugService;

        public CreateBoardColumnCommandHandler(IDataContext dataContext, IMappingService mappingService, ISlugService slugService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
            this.slugService = slugService;
        }

        public async Task<Dto.BoardColumn> HandleAsync(CreateBoardColumnCommand command)
        {
            var boardColumn = mappingService.Map<Domain.BoardColumn>(command.BoardColumn);
            boardColumn.Slug = slugService.Slugify(boardColumn.Name);

            if (dataContext.Set<Domain.BoardColumn>().Any(x => x.Slug == boardColumn.Slug))
            {
                throw new CreateBoardColumnCommandSlugExistsException();
            }

            var board = dataContext.Set<Domain.Board>().FirstOrDefault(x => x.Slug == command.BoardSlug);
            if (board == null)
            {
                throw new BoardNotFoundException();
            }

            dataContext.Set<Domain.BoardColumn>().Add(boardColumn);
            board.Columns.Add(boardColumn);

            await dataContext.SaveChangesAsync();
            return mappingService.Map<Dto.BoardColumn>(boardColumn);
        }
    }
}