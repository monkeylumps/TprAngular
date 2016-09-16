using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class SearchBoardColumnsQueryHandler : IQueryHandler<SearchBoardColumnsQuery, BoardColumnCollection>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public SearchBoardColumnsQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardColumnCollection> HandleAsync(SearchBoardColumnsQuery query)
        {
            if (!await dataContext.Set<BoardEntity>().AnyAsync(x => x.Slug == query.BoardSlug))
            {
                throw new BoardNotFoundException();
            }

            var boardColumnEntities = await dataContext.Set<BoardColumnEntity>().Where(x => x.BoardEntity.Slug == query.BoardSlug).ToListAsync();

            return new BoardColumnCollection
            {
                Items = boardColumnEntities.Select(x => mappingService.Map<BoardColumn>(x)).ToList()
            };
        }
    }
}