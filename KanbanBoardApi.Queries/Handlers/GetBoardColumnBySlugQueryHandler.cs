using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class GetBoardColumnBySlugQueryHandler : IQueryHandler<GetBoardColumnBySlugQuery, BoardColumn>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public GetBoardColumnBySlugQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardColumn> HandleAsync(GetBoardColumnBySlugQuery query)
        {
            var boardColumn =
                await dataContext.Set<BoardEntity>()
                    .Where(x => x.Slug == query.BoardSlug)
                    .Select(x => x.Columns.FirstOrDefault(y => y.Slug == query.BoardColumnSlug))
                    .FirstOrDefaultAsync();

            if (boardColumn == null)
            {
                return null;
            }

            return mappingService.Map<BoardColumn>(boardColumn);
        }
    }
}