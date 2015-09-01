using System.Data.Entity;
using System.Threading.Tasks;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;

namespace KanbanBoardApi.Queries.Handlers
{
    public class GetBoardTaskByIdQueryHandler : IQueryHandler<GetBoardTaskByIdQuery, BoardTask>
    {
        private readonly IDataContext dataContext;
        private readonly IMappingService mappingService;

        public GetBoardTaskByIdQueryHandler(IDataContext dataContext, IMappingService mappingService)
        {
            this.dataContext = dataContext;
            this.mappingService = mappingService;
        }

        public async Task<BoardTask> HandleAsync(GetBoardTaskByIdQuery query)
        {
            var boardTaskEntity = await dataContext.Set<BoardTaskEntity>().FirstOrDefaultAsync(x => x.Id == query.TaskId);

            if (boardTaskEntity == null)
            {
                return null;
            }

            return mappingService.Map<BoardTask>(boardTaskEntity);
        }
    }
}