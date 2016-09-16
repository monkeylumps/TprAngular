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
    public class CreateBoardCommandHandler : ICommandHandler<CreateBoardCommand, Board>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;
        private readonly ISlugService slugService;

        public CreateBoardCommandHandler(IDataContext dataContext, IMappingService mappingService,
            ISlugService slugService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
            this.slugService = slugService;
        }

        public async Task<Board> HandleAsync(CreateBoardCommand command)
        {
            var board = mappingService.Map<BoardEntity>(command.Board);
            board.Slug = slugService.Slugify(board.Name);

            if (dataContext.Set<BoardEntity>().Any(x => x.Slug == board.Slug))
            {
                throw new CreateBoardCommandSlugExistsException();
            }

            dataContext.Set<BoardEntity>().Add(board);
            await dataContext.SaveChangesAsync();
            return mappingService.Map<Board>(board);
        }
    }
}