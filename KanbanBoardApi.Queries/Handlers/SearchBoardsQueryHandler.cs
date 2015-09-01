using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class SearchBoardsQueryHandler : IQueryHandler<SearchBoardsQuery, BoardCollection>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public SearchBoardsQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardCollection> HandleAsync(SearchBoardsQuery query)
        {
            var boards = await dataContext.Set<BoardEntity>().ToListAsync();

            return new BoardCollection
            {
                Items = boards.Select(x => mappingService.Map<Board>(x)).ToList()
            };
        }
    }
}