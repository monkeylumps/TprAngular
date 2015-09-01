using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class CreateBoardCommandHandlerTests
    {
        private CreateBoardCommandHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private Mock<ISlugService> mockSlugService;

        private void SetupCommandHandler(IList<BoardEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            mockSlugService = new Mock<ISlugService>();

            mockMappingService = new Mock<IMappingService>();
            handler = new CreateBoardCommandHandler(mockDataContext.Object, mockMappingService.Object,
                mockSlugService.Object);
        }

        [Fact]
        public async void GivenABoardWhenOkThenSaveChangedCalled()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>());
            var command = new CreateBoardCommand
            {
                Board = new Board()
            };

            mockMappingService.Setup(x => x.Map<BoardEntity>(It.IsAny<Board>())).Returns(new BoardEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenABoardWhenOkThenMappingBackToDto()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>());
            var command = new CreateBoardCommand
            {
                Board = new Board()
            };

            mockMappingService.Setup(x => x.Map<BoardEntity>(It.IsAny<Board>())).Returns(new BoardEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<Board>(It.IsAny<BoardEntity>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardWhenOkThenBoardNameSlugified()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>());
            var command = new CreateBoardCommand
            {
                Board = new Board()
            };

            mockMappingService.Setup(x => x.Map<BoardEntity>(It.IsAny<Board>())).Returns(new BoardEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockSlugService.Verify(x => x.Slugify(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardWhenSlugAlreadyExistsThenThrowException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity
                {
                    Id = 1,
                    Name = "test",
                    Slug = "test"
                }
            });
            var command = new CreateBoardCommand
            {
                Board = new Board
                {
                    Slug = "test"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardEntity>(It.IsAny<Board>())).Returns(new BoardEntity
            {
                Name = "test",
                Slug = "test"
            });

            mockSlugService.Setup(x => x.Slugify(It.IsAny<string>())).Returns("test");

            // Act & Assert
            await Assert.ThrowsAsync<CreateBoardCommandSlugExistsException>(() => handler.HandleAsync(command));
        }
    }
}