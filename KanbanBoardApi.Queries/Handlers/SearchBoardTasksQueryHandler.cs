using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class SearchBoardTasksQueryHandler : IQueryHandler<SearchBoardTasksQuery, BoardTaskCollection>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public SearchBoardTasksQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardTaskCollection> HandleAsync(SearchBoardTasksQuery query)
        {
            var linqQuery = dataContext.Set<BoardTaskEntity>()
                .Include(x => x.BoardColumnEntity)
                .Where(x => x.BoardColumnEntity.BoardEntity.Slug == query.BoardSlug);

            if (!string.IsNullOrWhiteSpace(query.BoardColumnSlug))
            {
                linqQuery = linqQuery.Where(x => x.BoardColumnEntity.Slug == query.BoardColumnSlug);
            }

            var boardTasks = await linqQuery.ToListAsync();

            return new BoardTaskCollection
            {
                Items = boardTasks.Select(x => mappingService.Map<BoardTask>(x)).ToList()
            };
        }
    }
}