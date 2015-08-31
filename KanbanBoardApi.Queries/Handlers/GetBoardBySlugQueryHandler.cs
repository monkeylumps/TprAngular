using System.Data.Entity;
using System.Threading.Tasks;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class GetBoardBySlugQueryHandler : IQueryHandler<GetBoardBySlugQuery, Board>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public GetBoardBySlugQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<Board> HandleAsync(GetBoardBySlugQuery query)
        {
            var board = await dataContext.Set<Domain.Board>().FirstOrDefaultAsync(x => x.Slug == query.BoardSlug);

            if (board == null)
            {
                return null;
            }

            return mappingService.Map<Dto.Board>(board);
        }
    }
}