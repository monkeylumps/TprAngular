using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class UpdateBoardTaskCommandHandlerTests
    {
        private UpdateBoardTaskCommandHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupCommandHandler(IList<BoardTaskEntity> boardTasks)
        {
            mockDataContext = new Mock<IDataContext>();
            var fakeDbSet = new FakeDbSet<BoardTaskEntity>();
            boardTasks.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();
            handler = new UpdateBoardTaskCommandHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenCommandWhenTaskDoesNotExistThenThrowBoardTaskNotFoundException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardTaskEntity>());
            var command = new UpdateBoardTaskCommand();

            // Act & Assert
            await Assert.ThrowsAsync<BoardTaskNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenAttachedSaveChangesCalled()
        {
            // Arrange
            var boardEntry = new BoardTaskEntity
            {
                Id = 1
            };

            SetupCommandHandler(new List<BoardTaskEntity>
            {
                boardEntry
            });
            var command = new UpdateBoardTaskCommand
            {
                BoardTask = new BoardTask
                {
                    Id = 1
                }
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SetModified(boardEntry), Times.Once);
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenMapDtoToEntity()
        {
            // Arrange
            var boardTaskEntity = new BoardTaskEntity
            {
                Id = 1
            };
            SetupCommandHandler(new List<BoardTaskEntity>
            {
                boardTaskEntity
            });
            var command = new UpdateBoardTaskCommand
            {
                BoardTask = new BoardTask
                {
                    Id = 1
                }
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map(It.IsAny<BoardTask>(), It.IsAny<BoardTaskEntity>()), Times.Once);
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenMappedBackToDto()
        {
            // Arrange
            SetupCommandHandler(new List<BoardTaskEntity>
            {
                new BoardTaskEntity
                {
                    Id = 1
                }
            });
            var command = new UpdateBoardTaskCommand
            {
                BoardTask = new BoardTask
                {
                    Id = 1
                }
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<BoardTask>(It.IsAny<BoardTaskEntity>()), Times.Once);
        }
    }
}