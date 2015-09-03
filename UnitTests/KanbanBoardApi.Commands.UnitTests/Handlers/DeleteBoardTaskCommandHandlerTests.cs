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
    public class DeleteBoardTaskCommandHandlerTests
    {
        private Mock<IDataContext> mockDataContext;

        private DeleteBoardTaskCommandHandler handler;

        private void SetupCommandHandler(IList<BoardEntity> boards, IList<BoardTaskEntity> boardTasks)
        {
            mockDataContext = new Mock<IDataContext>();

            var fakeBoardDbSet = new FakeDbSet<BoardEntity>();
            boards.ToList().ForEach(x => fakeBoardDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeBoardDbSet);

            var fakeBoardTaskDbSet = new FakeDbSet<BoardTaskEntity>();
            boardTasks.ToList().ForEach(x => fakeBoardTaskDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(fakeBoardTaskDbSet);

            handler = new DeleteBoardTaskCommandHandler(mockDataContext.Object);
        }

        [Fact]
        public async void GivenCommandWhenBoardDoesNotExistsThenBoardNotFoundThrown()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>(), new List<BoardTaskEntity>());
            var command = new DeleteBoardTaskCommand();

            // Act & Assert
            await Assert.ThrowsAsync<BoardNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardTaskDoesNotExistsThenBoardTaskNotFoundThrown()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity {Slug = "board-name"}
            }, new List<BoardTaskEntity>());

            var command = new DeleteBoardTaskCommand
            {
                BoardSlug = "board-name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardTaskNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardAndBoardTaskExistThenBoardTaskRemovedAndSaveChangedCalled()
        {
            // Arrange
            var boardTaskEntity = new BoardTaskEntity { Id = 1 };
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity {Slug = "board-name"}
            }, new List<BoardTaskEntity> { boardTaskEntity });

            var command = new DeleteBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTaskId = 1
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
            mockDataContext.Verify(x => x.Delete(boardTaskEntity), Times.Once());
        }
    }
}