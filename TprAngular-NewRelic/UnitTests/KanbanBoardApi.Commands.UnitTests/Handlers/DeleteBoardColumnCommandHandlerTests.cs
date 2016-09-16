using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Domain;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class DeleteBoardColumnCommandHandlerTests
    {
        private Mock<IDataContext> mockDataContext;
        private DeleteBoardColumnCommandHandler handler;

        private void SetupHandler(IList<BoardEntity> boards,
            IList<BoardColumnEntity> boardColumns,
            IList<BoardTaskEntity> boardTasks)
        {
            mockDataContext = new Mock<IDataContext>();

            var fakeBoardDbSet = new FakeDbSet<BoardEntity>();
            boards.ToList().ForEach(x => fakeBoardDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeBoardDbSet);

            var fakeBoardColumnDbSet = new FakeDbSet<BoardColumnEntity>();
            boardColumns.ToList().ForEach(x => fakeBoardColumnDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardColumnEntity>()).Returns(fakeBoardColumnDbSet);

            var fakeBoardTaskDbSet = new FakeDbSet<BoardTaskEntity>();
            boardTasks.ToList().ForEach(x => fakeBoardTaskDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(fakeBoardTaskDbSet);

            handler = new DeleteBoardColumnCommandHandler(mockDataContext.Object);
        }
        [Fact]
        public async void GivenCommandWhenBoardDoesNotExistThenBoardNotFoundExceptionThrown()
        {
            // Arrange
            SetupHandler(new List<BoardEntity>(), new List<BoardColumnEntity>(), new List<BoardTaskEntity>());
            var command = new DeleteBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardColumnDoesNotExistThenBoardColumnNotFoundExceptionThrown()
        {
            // Arrange
            SetupHandler(new List<BoardEntity>
            {
                new BoardEntity { Slug="board-name" }
            }, new List<BoardColumnEntity>(), new List<BoardTaskEntity>());
            var command = new DeleteBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardColumnNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardColumnHasTasksThenBoardColumnNotEmptyExceptionThrown()
        {
            // Arrange
            var boardEntity = new BoardEntity {Slug = "board-name"};
            var boardColumnEntity = new BoardColumnEntity { Id = 1, Slug = "board-column-name", BoardEntity = boardEntity };
            SetupHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity>
            {
                boardColumnEntity
            }, new List<BoardTaskEntity>
            {
                new BoardTaskEntity {BoardColumnEntity = boardColumnEntity}
            });

            var command = new DeleteBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardColumnNotEmptyException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardColumnExisttsAndIsEmptyThenBoardColumnRemovedAndSaveChangesCalled()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };
            var boardColumnEntity = new BoardColumnEntity { Id = 1, Slug = "board-column-name", BoardEntity = boardEntity };
            SetupHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity>
            {
                boardColumnEntity
            }, new List<BoardTaskEntity>());

            var command = new DeleteBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name"
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
            mockDataContext.Verify(x => x.Delete(boardColumnEntity), Times.Once());
        }
    }
}