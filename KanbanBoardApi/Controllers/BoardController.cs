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
    [RoutePrefix("boards")]
    public class BoardController : ApiController
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IHyperMediaFactory hyperMediaFactory;

        public BoardController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IHyperMediaFactory hyperMediaFactory)
        {
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.hyperMediaFactory = hyperMediaFactory;
        }

        /// <summary>
        /// Creates a new Kanban Board
        /// </summary>
        /// <param name="board">Kanban board to create</param>
        /// <response code="409"/>
        /// <response code="400"/>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Board))]
        public async Task<IHttpActionResult> Post(Board board)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await commandDispatcher.HandleAsync<CreateBoardCommand, Board>(
                new CreateBoardCommand
                {
                    Board = board
                });

                hyperMediaFactory.Apply(result);

                return Created(hyperMediaFactory.GetLink(result, Link.SELF), result);
            }
            catch (CreateBoardCommandSlugExistsException)
            {
                return Conflict();
            }
        }

        [Route("{slug}", Name="BoardsGet")]
        [HttpGet]
        [ResponseType(typeof(Board))]
        public async Task<IHttpActionResult> Get(string slug)
        {
            var result = await queryDispatcher.HandleAsync<GetBoardBySlugQuery, Board>(new GetBoardBySlugQuery
            {
                Slug = slug
            });

            if (result == null)
            {
                return NotFound();
            }

            hyperMediaFactory.Apply(result);

            return Ok(result);
        }

        [HttpGet]
        [Route("", Name="BoardsSearch")]
        [ResponseType(typeof(BoardCollection))]
        public async Task<IHttpActionResult> Search()
        {
            var result = await queryDispatcher.HandleAsync<SearchBoardsQuery, BoardCollection>(new SearchBoardsQuery());

            hyperMediaFactory.Apply(result);

            return Ok(result);
        }
    }
}