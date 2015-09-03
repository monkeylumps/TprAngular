using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class UpdateBoardColumnCommandHandlerTests
    {
        private UpdateBoardColumnCommandHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupCommandHandler(IList<BoardEntity> boards, IList<BoardColumnEntity> boardColumns)
        {
            mockDataContext = new Mock<IDataContext>();
            var fakeDbSet = new FakeDbSet<BoardEntity>();
            boards.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            var fakeColumnDbSet = new FakeDbSet<BoardColumnEntity>();
            boardColumns.ToList().ForEach(x => fakeColumnDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardColumnEntity>()).Returns(fakeColumnDbSet);

            mockMappingService = new Mock<IMappingService>();
            handler = new UpdateBoardColumnCommandHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenCommandWhenBoardDoesNotExistThenThrowBoardNotFoundException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>(), new List<BoardColumnEntity>());
            var command = new UpdateBoardColumnCommand();

            // Act & Assert
            await Assert.ThrowsAsync<BoardNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenBoardColumnDoesNotExistThenThrowBoardColumnNotFoundException()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };

            SetupCommandHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity> ());

            var command = new UpdateBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name",
                BoardColumn = new BoardColumn()
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardColumnNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenAttachedSaveChangesCalled()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };
            var boardColumnEntity = new BoardColumnEntity
            {
                Slug = "board-column-name",
                BoardEntity = boardEntity
            };

            SetupCommandHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity> { boardColumnEntity });

            var command = new UpdateBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name",
                BoardColumn = new BoardColumn()
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SetModified(boardColumnEntity), Times.Once);
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsButBoardColumnDoesNotThenThrowBoardColumnNotFoundException()
        {
            // Arrange
            var boardEntity = new BoardEntity {Slug = "board-name"};

            SetupCommandHandler(new List<BoardEntity>{ boardEntity }, new List<BoardColumnEntity> {  } );

            var command = new UpdateBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name",
                BoardColumn = new BoardColumn()
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardColumnNotFoundException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenMapDtoToEntity()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };
            var boardColumnEntity = new BoardColumnEntity
            {
                Slug = "board-column-name",
                BoardEntity = boardEntity
            };

            SetupCommandHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity> { boardColumnEntity });

            var command = new UpdateBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name",
                BoardColumn = new BoardColumn
                {
                    Slug = "board-column-name"
                }
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map(It.IsAny<BoardColumn>(), It.IsAny<BoardColumnEntity>()), Times.Once);
        }

        [Fact]
        public async void GivenCommandWhenTaskExistsThenMappedBackToDto()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };
            var boardColumnEntity = new BoardColumnEntity
            {
                Slug = "board-column-name",
                BoardEntity = boardEntity
            };

            SetupCommandHandler(new List<BoardEntity> { boardEntity }, new List<BoardColumnEntity> { boardColumnEntity });

            var command = new UpdateBoardColumnCommand
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name",
                BoardColumn = new BoardColumn()
            };

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<BoardColumn>(It.IsAny<BoardColumnEntity>()), Times.Once);
        }
    }
}