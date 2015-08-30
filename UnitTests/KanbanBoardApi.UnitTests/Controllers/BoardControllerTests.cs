using System.Web.Http.Results;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Controllers;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using Moq;
using Xunit;
using KanbanBoardApi.Queries;

namespace KanbanBoardApi.UnitTests.Controllers
{
    public class BoardControllerTests
    {
        private Mock<ICommandDispatcher> mockCommandDispatcher;
        private Mock<IQueryDispatcher> mockQueryDispatcher;
        private Mock<IHyperMediaFactory> mockHyperMediaFactory;
        private BoardController controller;

        private void SetupController()
        {
            mockCommandDispatcher = new Mock<ICommandDispatcher>();
            mockQueryDispatcher = new Mock<IQueryDispatcher>();
            mockHyperMediaFactory = new Mock<IHyperMediaFactory>();
            controller = new BoardController(
                    mockCommandDispatcher.Object,
                    mockQueryDispatcher.Object,
                    mockHyperMediaFactory.Object);
        }

        [Fact]
        public async void GivenABoardWhenDataIsValidThenCreatedOkResultReturned()
        {
            // Arrange
            SetupController();

            var board = new Board
            {
                Name = "new board"
            };

            mockCommandDispatcher.Setup(x => x.HandleAsync<CreateBoardCommand, Board>(It.IsAny<CreateBoardCommand>()))
                .ReturnsAsync(new Board());
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            var createdNegotiatedContentResult = await controller.Post(board) as CreatedNegotiatedContentResult<Board>;

            // Assert
            Assert.NotNull(createdNegotiatedContentResult);
        }

        [Fact]
        public async void GivenABoardWhenDataIsValidThenHyperMediaSet()
        {
            // Arrange
            SetupController();

            var board = new Board
            {
                Name = "new board"
            };

            mockCommandDispatcher.Setup(x => x.HandleAsync<CreateBoardCommand, Board>(It.IsAny<CreateBoardCommand>()))
                .ReturnsAsync(new Board());
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            var createdNegotiatedContentResult = await controller.Post(board) as CreatedNegotiatedContentResult<Board>;

            // Assert
            Assert.NotNull(createdNegotiatedContentResult);
            mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<Board>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardWhenDataIsValidThenCreateBoardCommandCalled()
        {
            // Arrange
            SetupController();

            var board = new Board
            {
                Name = "new board"
            };

            mockCommandDispatcher.Setup(x => x.HandleAsync<CreateBoardCommand, Board>(It.IsAny<CreateBoardCommand>()))
                .ReturnsAsync(new Board());
            mockHyperMediaFactory.Setup(x => x.GetLink(It.IsAny<IHyperMediaItem>(), It.IsAny<string>()))
                .Returns("http://fake-url/");

            // Act
            var createdNegotiatedContentResult = await controller.Post(board) as CreatedNegotiatedContentResult<Board>;

            // Assert
            Assert.NotNull(createdNegotiatedContentResult);
            mockCommandDispatcher.Verify(x => x.HandleAsync<CreateBoardCommand, Board>(It.IsAny<CreateBoardCommand>()),
                Times.Once);
        }

        [Fact]
        public async void GiveABoardWhenDataIsNotValidThenInvalidModelStateResultReturned()
        {
            // Arrange
            SetupController();

            var board = new Board();

            // force a validation error
            controller.ModelState.AddModelError("error", "error");

            // Act
            var invalidModelStateResult = await controller.Post(board) as InvalidModelStateResult;

            // Assert
            Assert.NotNull(invalidModelStateResult);
        }

        [Fact]
        public async void GiveASlugWhenBoardExistsThenBoardIsReturned()
        {
            // Arrange
            SetupController();
            const string slug = "test-slug";
            mockQueryDispatcher.Setup(x => x.HandleAsync<GetBoardBySlugQuery, Board>(It.IsAny<GetBoardBySlugQuery>()))
                .ReturnsAsync(new Board());

            // Act
            var okNegotiatedContentResult = await controller.Get(slug) as OkNegotiatedContentResult<Board>;

            // Assert
            Assert.NotNull(okNegotiatedContentResult);
            Assert.NotNull(okNegotiatedContentResult.Content);
        }

        [Fact]
        public async void GivenASlugWhenBoardExistsThenGetBoardBySlugQueryCalled()
        {
            // Arrange
            SetupController();
            const string slug = "test-slug";

            // Act
            await controller.Get(slug);

            // Assert
            mockQueryDispatcher.Verify(x => x.HandleAsync<GetBoardBySlugQuery, Board>(It.IsAny<GetBoardBySlugQuery>()), Times.Once);
        }


        [Fact]
        public async void GiveASlugWhenBoardExistsThenHypermediaSet()
        {
            // Arrange
            SetupController();
            const string slug = "test-slug";
            mockQueryDispatcher.Setup(x => x.HandleAsync<GetBoardBySlugQuery, Board>(It.IsAny<GetBoardBySlugQuery>()))
                .ReturnsAsync(new Board());

            // Act
            await controller.Get(slug);

            // Assert
            mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async void GivenASlugWhenBoardDoesNotExistsThenNotFoundReturned()
        {
            // Arrange
            SetupController();
            const string slug = "test-slug";

            // Act
            var notFoundResult = await controller.Get(slug) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
        }


        [Fact]
        public async void GiveDefaultValuesWhenBoardsExistABoardCollectionIsReturned()
        {
            // Arrange
            SetupController();

            // Act
            var okNegotiatedContentResult = await controller.Search() as OkNegotiatedContentResult<BoardCollection>;

            // Assert
            Assert.NotNull(okNegotiatedContentResult);
        }

        [Fact]
        public async void GivenDefaultvaluesWhenBoardsExistsThenSearchBoardsQueryCsalled()
        {
            // Arrange
            SetupController();
            mockQueryDispatcher.Setup(
                x => x.HandleAsync<SearchBoardsQuery, BoardCollection>(It.IsAny<SearchBoardsQuery>()))
                .ReturnsAsync(new BoardCollection());

            // Act
            await controller.Search();

            // Assert
            mockQueryDispatcher.Verify(x => x.HandleAsync<SearchBoardsQuery, BoardCollection>(It.IsAny<SearchBoardsQuery>()), Times.Once);
        }

        [Fact]
        public async void GivenDefaultvaluesWhenBoardsExistsThenHypermediaSet()
        {
            // Arrange
            SetupController();
            mockQueryDispatcher.Setup(
                x => x.HandleAsync<SearchBoardsQuery, BoardCollection>(It.IsAny<SearchBoardsQuery>()))
                .ReturnsAsync(new BoardCollection());

            // Act
            await controller.Search();

            // Assert
            mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<object>()), Times.Once);
        }
    }
}