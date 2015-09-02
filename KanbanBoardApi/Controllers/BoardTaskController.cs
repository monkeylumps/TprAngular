using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.Queries;

namespace KanbanBoardApi.Controllers
{
    public class BoardTaskController : ApiController
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IHyperMediaFactory hyperMediaFactory;
        private readonly IQueryDispatcher queryDispatcher;

        public BoardTaskController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            IHyperMediaFactory hyperMediaFactory)
        {
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.hyperMediaFactory = hyperMediaFactory;
        }

        [HttpPost]
        [Route("{boardSlug}/columns/{boardColumnSlug}", Name = "BoardTaskPost")]
        [ResponseType(typeof(BoardTask))]
        public async Task<IHttpActionResult> Post(string boardSlug, string boardColumnSlug, BoardTask boardTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await commandDispatcher.HandleAsync<CreateBoardTaskCommand, BoardTask>(new CreateBoardTaskCommand
                {
                    BoardSlug = boardSlug,
                    BoardColumnSlug = boardColumnSlug,
                    BoardTask = boardTask
                });

                hyperMediaFactory.Apply(result);

                return Created(hyperMediaFactory.GetLink(result, Link.SELF), result);
            }
            catch (BoardColumnNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Route("{boardSlug}/tasks/{taskId:int}", Name = "BoardTaskPut")]
        [ResponseType(typeof(BoardTask))]
        public async Task<IHttpActionResult> Put(string boardSlug, int taskId, BoardTask boardTask)
        {
            try
            {
                var result = await commandDispatcher.HandleAsync<UpdateBoardTaskCommand, BoardTask>(new UpdateBoardTaskCommand
                {
                    BoardSlug = boardSlug,
                    BoardTask = boardTask
                });

                hyperMediaFactory.Apply(result);

                return Ok(result);
            }
            catch (BoardTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{boardSlug}/tasks/{taskId:int}", Name = "BoardTaskGet")]
        [ResponseType(typeof(BoardTask))]
        public async Task<IHttpActionResult> Get(string boardSlug, int taskId)
        {
            var boardTask = await queryDispatcher.HandleAsync<GetBoardTaskByIdQuery, BoardTask>(new GetBoardTaskByIdQuery
            {
                BoardSlug = boardSlug,
                TaskId = taskId
            });

            if (boardTask == null)
            {
                return NotFound();
            }

            hyperMediaFactory.Apply(boardTask);

            return Ok(boardTask);
        }

        [HttpGet]
        [Route("{boardSlug}/tasks", Name = "BoardTasksSearch")]
        [Route("{boardSlug}/columns/{boardColumnSlug}/tasks", Name = "BoardTaskByBoardColumnSearch")]
        [ResponseType(typeof(BoardTask))]
        public async Task<IHttpActionResult> Search(string boardSlug, string boardColumnSlug = "")
        {
            try
            {
                var result = await queryDispatcher.HandleAsync<SearchBoardTasksQuery, BoardTaskCollection>(new SearchBoardTasksQuery
                {
                    BoardSlug = boardSlug,
                    BoardColumnSlug = boardColumnSlug
                });

                hyperMediaFactory.Apply(result);

                return Ok(result);
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
        }
    }
}