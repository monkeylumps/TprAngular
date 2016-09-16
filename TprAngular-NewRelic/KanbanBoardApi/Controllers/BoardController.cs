﻿using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.Dto;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.Queries;

namespace KanbanBoardApi.Controllers
{
    [RoutePrefix("boards")]
    public class BoardController : ApiController
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IHyperMediaFactory hyperMediaFactory;
        private readonly IQueryDispatcher queryDispatcher;

        public BoardController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            IHyperMediaFactory hyperMediaFactory)
        {
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.hyperMediaFactory = hyperMediaFactory;
        }

        /// <summary>
        /// Creates a new Kanban Board
        /// </summary>
        /// <param name="board">Kanban board to create</param>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof (Board))]
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

        /// <summary>
        /// Gets a Kanban board by it's Slug
        /// </summary>
        /// <param name="boardSlug">Slug of the board to return</param>
        [Route("{boardSlug}", Name = "BoardsGet")]
        [HttpGet]
        [ResponseType(typeof (Board))]
        public async Task<IHttpActionResult> Get(string boardSlug)
        {
            var result = await queryDispatcher.HandleAsync<GetBoardBySlugQuery, Board>(new GetBoardBySlugQuery
            {
                BoardSlug = boardSlug
            });

            if (result == null)
            {
                return NotFound();
            }

            hyperMediaFactory.Apply(result);

            return Ok(result);
        }

        /// <summary>
        /// Searchs for available Kanban Boards
        /// </summary>
        [HttpGet]
        [Route("", Name = "BoardSearch")]
        [ResponseType(typeof (BoardCollection))]
        public async Task<IHttpActionResult> Search()
        {
            var result = await queryDispatcher.HandleAsync<SearchBoardsQuery, BoardCollection>(new SearchBoardsQuery());

            hyperMediaFactory.Apply(result);

            return Ok(result);
        }
    }
}