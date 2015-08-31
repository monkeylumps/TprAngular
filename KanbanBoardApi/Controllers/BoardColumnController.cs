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
    public class BoardColumnController : ApiController
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IHyperMediaFactory hyperMediaFactory;
        private readonly IQueryDispatcher queryDispatcher;

        public BoardColumnController(ICommandDispatcher commandDispatcher, IHyperMediaFactory hyperMediaFactory, IQueryDispatcher queryDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
            this.hyperMediaFactory = hyperMediaFactory;
            this.queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        [Route("{boardSlug}/columns/{boardColumnSlug}", Name= "BoardColumnGet")]
        [ResponseType(typeof(BoardColumn))]
        public async Task<IHttpActionResult> Get(string boardSlug, string boardColumnSlug)
        {
            var boardColumn = await queryDispatcher.HandleAsync<GetBoardColumnBySlugQuery, BoardColumn>(new GetBoardColumnBySlugQuery
            {
                BoardSlug = boardSlug,
                BoardColumnSlug = boardColumnSlug
            });

            if (boardColumn == null)
            {
                return NotFound();
            }

            hyperMediaFactory.Apply(boardColumn);

            return Ok(boardColumn);
        }

        [HttpPost]
        [Route("{boardSlug}/columns")]
        [ResponseType(typeof(BoardColumn))]
        public async Task<IHttpActionResult> Post(string boardSlug, BoardColumn boardColumn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result =
                    await commandDispatcher.HandleAsync<CreateBoardColumnCommand, BoardColumn>(new CreateBoardColumnCommand
                    {
                        BoardSlug = boardSlug,
                        BoardColumn = boardColumn
                    });

                if (result == null)
                {
                    return NotFound();
                }

                hyperMediaFactory.Apply(result);

                return Created(hyperMediaFactory.GetLink(result, Link.SELF), result);
            }
            catch (CreateBoardColumnCommandSlugExistsException)
            {
                return Conflict();
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
        }
    }
}