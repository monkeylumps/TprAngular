using System.Web.Http.Results;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Controllers;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.Queries;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.Controllers
{
    public class BoardColumnControllerrTests
    {
        private BoardColumnController controller;
        private Mock<ICommandDispatcher> mockCommandDispatcher;
        private Mock<IHyperMediaFactory> mockHyperMediaFactory;
        private Mock<IQueryDispatcher> mockQueryDispatcher;

        private void SetupController()
        {
            mockHyperMediaFactory = new Mock<IHyperMediaFactory>();
            mockCommandDispatcher = new Mock<ICommandDispatcher>();
            mockQueryDispatcher = new Mock<IQueryDispatcher>();
            controller = new BoardColumnController(mockCommandDispatcher.Object, mockHyperMediaFactory.Object,
                mockQueryDispatcher.Object);
        }


        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardExistsThenCreateColumnOnBoard()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            mockCommandDispatcher.Setup(
                x => x.HandleAsync<CreateBoardColumnCommand, BoardColumn>(It.IsAny<CreateBoardColumnCommand>()))
                .ReturnsAsync(new BoardColumn());
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            var createdNegotiatedContentResult =
                await controller.Post(boardSlug, column) as CreatedNegotiatedContentResult<BoardColumn>;

            // Assert
            Assert.NotNull(createdNegotiatedContentResult);
        }


        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardExistsThenCreateBoardColumnCommandCalled()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            await controller.Post(boardSlug, column);

            // Assert
            mockCommandDispatcher.Verify(
                x =>
                    x.HandleAsync<CreateBoardColumnCommand, BoardColumn>(
                        It.Is<CreateBoardColumnCommand>(y => y.BoardColumn == column && y.BoardSlug == boardSlug)),
                Times.Once);
        }

        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardExistsThenHyperMediaSet()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            mockCommandDispatcher.Setup(
                x => x.HandleAsync<CreateBoardColumnCommand, BoardColumn>(It.IsAny<CreateBoardColumnCommand>()))
                .ReturnsAsync(new BoardColumn());
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            await controller.Post(boardSlug, column);

            // Assert
            mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<BoardColumn>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardColumnIsInvalidThenReturnInvalidModelState()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            controller.ModelState.AddModelError("error", "error");

            // Act
            var invalidModelStateResult = await controller.Post(boardSlug, column) as InvalidModelStateResult;

            // Assert
            Assert.NotNull(invalidModelStateResult);
        }


        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardIsNotFoundThenReturnNotFound()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();

            // Act
            var notFoundResult = await controller.Post(boardSlug, column) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }


        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenSlugForBoardColumnexistsThenConflictReturned()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            mockCommandDispatcher.Setup(
                x => x.HandleAsync<CreateBoardColumnCommand, BoardColumn>(It.IsAny<CreateBoardColumnCommand>()))
                .Throws<CreateBoardColumnCommandSlugExistsException>();

            // Act
            var conflictResult = await controller.Post(boardSlug, column) as ConflictResult;

            // Assert
            Assert.NotNull(conflictResult);
        }


        [Fact]
        public async void GivenABoardSlugAndBoardColumnWhenBoardDoesNotExistThenNotFound()
        {
            // Arrange
            SetupController();
            var boardSlug = "test";
            var column = new BoardColumn();
            mockCommandDispatcher.Setup(
                x => x.HandleAsync<CreateBoardColumnCommand, BoardColumn>(It.IsAny<CreateBoardColumnCommand>()))
                .Throws<BoardNotFoundException>();

            // Act
            var notFoundResult = await controller.Post(boardSlug, column) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }


        [Fact]
        public async void GivenASlugAndABoardColumnSlugWhenBothExistThenOkResultIsReturned()
        {
            // Arrange
            SetupController();
            const string boardSlug = "column-name";
            const string boardColumnSlug = "board-column-name";
            mockQueryDispatcher.Setup(
                x =>
                    x.HandleAsync<GetBoardColumnBySlugQuery, BoardColumn>(It.IsAny<GetBoardColumnBySlugQuery>()))
                .ReturnsAsync(new BoardColumn());

            // Act
            var okNegotiatedContentResult =
                await controller.Get(boardSlug, boardColumnSlug) as OkNegotiatedContentResult<BoardColumn>;

            // Assert
            Assert.NotNull(okNegotiatedContentResult);
        }

        [Fact]
        public async void GivenASlugAndABoardColumnSlugWhenBothExistThenHyperMediaIsSet()
        {
            // Arrange
            SetupController();
            const string boardSlug = "column-name";
            const string boardColumnSlug = "board-column-name";
            mockQueryDispatcher.Setup(
                x =>
                    x.HandleAsync<GetBoardColumnBySlugQuery, BoardColumn>(It.IsAny<GetBoardColumnBySlugQuery>()))
                .ReturnsAsync(new BoardColumn());

            // Act
            await controller.Get(boardSlug, boardColumnSlug);

            // Assert
            mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<BoardColumn>()), Times.Once);
        }

        [Fact]
        public async void GivenASlugAndABoardColumnSlugWhenBothExistThenGetBoardColumBySlugCalled()
        {
            // Arrange
            SetupController();
            const string boardSlug = "column-name";
            const string boardColumnSlug = "board-column-name";
            mockQueryDispatcher.Setup(
                x =>
                    x.HandleAsync<GetBoardColumnBySlugQuery, BoardColumn>(It.IsAny<GetBoardColumnBySlugQuery>()))
                .ReturnsAsync(new BoardColumn());

            // Act
            await controller.Get(boardSlug, boardColumnSlug);

            // Assert
            mockQueryDispatcher.Verify(
                x =>
                    x.HandleAsync<GetBoardColumnBySlugQuery, BoardColumn>(It.Is<GetBoardColumnBySlugQuery>(
                        y => y.BoardSlug == boardSlug && y.BoardColumnSlug == boardColumnSlug)), Times.Once);
        }

        [Fact]
        public async void GivenASlugAndABoardColumnSlugWhenBoardColumnDoesNotExistsThenReturnNotFound()
        {
            // Arrange
            SetupController();
            const string boardSlug = "column-name";
            const string boardColumnSlug = "board-column-name";

            // Act
            var notFoundResult = await controller.Get(boardSlug, boardColumnSlug) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }
    }
}