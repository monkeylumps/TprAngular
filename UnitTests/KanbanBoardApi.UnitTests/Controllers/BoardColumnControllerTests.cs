using System.Web.Http.Results;
using KanbanBoardApi.Commands;
using KanbanBoardApi.Controllers;
using KanbanBoardApi.Dispatchers;
using KanbanBoardApi.Dto;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.Queries;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.Controllers
{
    public class BoardColumnControllerTests
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

        public class Post : BoardColumnControllerTests
        {
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
            public async void GivenABoardSlugAndBoardColumnWhenSlugForBoardColumnExistsThenConflictReturned()
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
        }

        public class Get : BoardColumnControllerTests
        {
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

        public class Delete : BoardColumnControllerTests
        {
            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugWhenBoardDoesNotExistThenNotFoundReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";

                mockCommandDispatcher.Setup(x => x.HandleAsync<DeleteBoardColumnCommand, string>(It.IsAny<DeleteBoardColumnCommand>()))
                    .Throws<BoardNotFoundException>();

                // Act
                var notFoundResult = await controller.Delete(boardSlug, boardColumnSlug) as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugWhenBoardColumnDoesNotExistThenNotFoundReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";

                mockCommandDispatcher.Setup(x => x.HandleAsync<DeleteBoardColumnCommand, string>(It.IsAny<DeleteBoardColumnCommand>()))
                    .Throws<BoardColumnNotFoundException>();

                // Act
                var notFoundResult = await controller.Delete(boardSlug, boardColumnSlug) as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugWhenBoardTasksStillExistsForBoardThenBadRequestReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";

                mockCommandDispatcher.Setup(x => x.HandleAsync<DeleteBoardColumnCommand, string>(It.IsAny<DeleteBoardColumnCommand>()))
                    .Throws<BoardColumnNotEmptyException>();

                // Act
                var badRequestErrorMessageResult = await controller.Delete(boardSlug, boardColumnSlug) as BadRequestErrorMessageResult;

                // Assert
                Assert.NotNull(badRequestErrorMessageResult);
                Assert.Equal("Board Tasks Are Still Assocaited To This Board Column", badRequestErrorMessageResult.Message);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugWhenDeletedThenOkReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";

                // Act
                var okResult = await controller.Delete(boardSlug, boardColumnSlug) as OkResult;

                // Assert
                Assert.NotNull(okResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugWhenDeletedThenDeleteBoardColumnCalled()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";

                // Act
                await controller.Delete(boardSlug, boardColumnSlug);

                // Assert
                mockCommandDispatcher.Verify(
                    x => x.HandleAsync<DeleteBoardColumnCommand, string>(
                        It.Is<DeleteBoardColumnCommand>(y => y.BoardSlug == boardSlug && y.BoardColumnSlug == boardColumnSlug)),Times.Once);
            }
        }

        public class Put : BoardColumnControllerTests
        {
            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugAndBoardWhenBoardDoesNotExistThenNotFoundReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";
                var boardColumn = new BoardColumn();

                mockCommandDispatcher.Setup(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(It.IsAny<UpdateBoardColumnCommand>()))
                    .Throws<BoardNotFoundException>();

                // Act
                var notFoundResult = await controller.Put(boardSlug, boardColumnSlug, boardColumn) as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugAndBoardWhenBoardColumnDoesNotExistThenNotFonndReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";
                var boardColumn = new BoardColumn();

                mockCommandDispatcher.Setup(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(It.IsAny<UpdateBoardColumnCommand>()))
                    .Throws<BoardColumnNotFoundException>();

                // Act
                var notFoundResult = await controller.Put(boardSlug, boardColumnSlug, boardColumn) as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugAndBoardWhenUpdatedThenOkResultReturned()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";
                var boardColumn = new BoardColumn();

                mockCommandDispatcher.Setup(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(It.IsAny<UpdateBoardColumnCommand>()))
                    .ReturnsAsync(boardColumn);

                // Act
                var okNegotiatedContentResult = await controller.Put(boardSlug, boardColumnSlug, boardColumn) as OkNegotiatedContentResult<BoardColumn>;

                // Assert
                Assert.NotNull(okNegotiatedContentResult);
            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugAndBoardWhenUpdatedThenUpdateBoardColumnCommandCalled()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";
                var boardColumn = new BoardColumn();

                mockCommandDispatcher.Setup(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(It.IsAny<UpdateBoardColumnCommand>()))
                    .ReturnsAsync(boardColumn);

                // Act
                await controller.Put(boardSlug, boardColumnSlug, boardColumn);

                // Assert
                mockCommandDispatcher.Verify(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(
                        It.Is<UpdateBoardColumnCommand>(y => y.BoardSlug == boardSlug && y.BoardColumnSlug == boardColumnSlug && y.BoardColumn == boardColumn)),
                    Times.Once);

            }

            [Fact]
            public async void GivenBoardSlugAndBoardColumnSlugAndBoardColumnWhenOkThenHyperMediaApplied()
            {
                // Arrange
                SetupController();
                const string boardSlug = "board-name";
                const string boardColumnSlug = "board-column-name";
                var boardColumn = new BoardColumn();

                mockCommandDispatcher.Setup(
                    x => x.HandleAsync<UpdateBoardColumnCommand, BoardColumn>(It.IsAny<UpdateBoardColumnCommand>()))
                    .ReturnsAsync(boardColumn);

                // Act
                await controller.Put(boardSlug, boardColumnSlug, boardColumn);

                // Assert
                mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<BoardColumn>()), Times.Once);
            }
        }

        public class Search : BoardColumnControllerTests
        {
            [Fact]
            public async void GiveDefaultValuesWhenBoardDoesNotExistThenNotFoundResultReturned()
            {
                // Arrange
                SetupController();
                string boardSlug = "board-name";
                mockQueryDispatcher.Setup(
                    x => x.HandleAsync<SearchBoardColumnsQuery, BoardColumnCollection>(It.IsAny<SearchBoardColumnsQuery>()))
                    .Throws<BoardNotFoundException>();

                // Act
                var notFoundResult = await controller.Search(boardSlug) as NotFoundResult;

                // Assert
                Assert.NotNull(notFoundResult);
            }

            [Fact]
            public async void GiveDefaultValuesWhenBoardExistThenBoardColumnCollectionIsReturned()
            {
                // Arrange
                SetupController();
                string boardSlug = "board-name";

                // Act
                var okNegotiatedContentResult = await controller.Search(boardSlug) as OkNegotiatedContentResult<BoardColumnCollection>;

                // Assert
                Assert.NotNull(okNegotiatedContentResult);
            }

            [Fact]
            public async void GivenDefaultvaluesWhenBoardsExistsThenSearchBoardColumnsQueryCalled()
            {
                // Arrange
                SetupController();
                string boardSlug = "board-name";
                mockQueryDispatcher.Setup(
                    x => x.HandleAsync<SearchBoardColumnsQuery, BoardColumnCollection>(It.IsAny<SearchBoardColumnsQuery>()))
                    .ReturnsAsync(new BoardColumnCollection());

                // Act
                await controller.Search(boardSlug);

                // Assert
                mockQueryDispatcher.Verify(
                    x => x.HandleAsync<SearchBoardColumnsQuery, BoardColumnCollection>(It.Is<SearchBoardColumnsQuery>(y => y.BoardSlug == boardSlug)), Times.Once);
            }

            [Fact]
            public async void GivenDefaultvaluesWhenBoardsExistsThenHypermediaSet()
            {
                // Arrange
                SetupController();
                string boardSlug = "board-name";
                mockQueryDispatcher.Setup(
                    x => x.HandleAsync<SearchBoardColumnsQuery, BoardColumnCollection>(It.IsAny<SearchBoardColumnsQuery>()))
                    .ReturnsAsync(new BoardColumnCollection());

                // Act
                await controller.Search(boardSlug);

                // Assert
                mockHyperMediaFactory.Verify(x => x.Apply(It.IsAny<object>()), Times.Once);
            }
        }
    }
}