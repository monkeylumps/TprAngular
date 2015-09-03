using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class CreateBoardColumnCommandHandlerTests
    {
        private CreateBoardColumnCommandHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private Mock<ISlugService> mockSlugService;

        private void SetupCommandHandler(IList<BoardColumnEntity> boardColumns, IList<BoardEntity> boards)
        {
            mockDataContext = new Mock<IDataContext>();

            var boardColumnDbSet = new FakeDbSet<BoardColumnEntity>();
            boardColumns.ToList().ForEach(x => boardColumnDbSet.Add(x));

            mockDataContext.Setup(x => x.Set<BoardColumnEntity>()).Returns(boardColumnDbSet);

            var boardDbSet = new FakeDbSet<BoardEntity>();
            boards.ToList().ForEach(x => boardDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(boardDbSet);

            mockSlugService = new Mock<ISlugService>();

            mockMappingService = new Mock<IMappingService>();
            handler = new CreateBoardColumnCommandHandler(mockDataContext.Object, mockMappingService.Object,
                mockSlugService.Object);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenSaveChangedCalled()
        {
            // Arrange
            SetupCommandHandler(new List<BoardColumnEntity>(), new List<BoardEntity>
            {
                new BoardEntity {Slug = "test"}
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<BoardColumnEntity>(It.IsAny<BoardColumn>()))
                .Returns(new BoardColumnEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenMappingBackToDto()
        {
            // Arrange
            SetupCommandHandler(new List<BoardColumnEntity>(), new List<BoardEntity>
            {
                new BoardEntity {Slug = "test"}
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<BoardColumnEntity>(It.IsAny<BoardColumn>()))
                .Returns(new BoardColumnEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<BoardColumn>(It.IsAny<BoardColumnEntity>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenBoardColumnNameSlugified()
        {
            // Arrange
            SetupCommandHandler(new List<BoardColumnEntity>(), new List<BoardEntity>
            {
                new BoardEntity {Slug = "test"}
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<BoardColumnEntity>(It.IsAny<BoardColumn>()))
                .Returns(new BoardColumnEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockSlugService.Verify(x => x.Slugify(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenSlugAlreadyExistsThenThrowCreateBoardColumnCommandSlugExistsException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardColumnEntity>
            {
                new BoardColumnEntity
                {
                    Id = 1,
                    Name = "test",
                    Slug = "test"
                }
            }, new List<BoardEntity>
            {
                new BoardEntity {Slug = "test"}
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new BoardColumn
                {
                    Slug = "test"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardColumnEntity>(It.IsAny<BoardColumn>()))
                .Returns(new BoardColumnEntity
                {
                    Name = "test",
                    Slug = "test"
                });

            mockSlugService.Setup(x => x.Slugify(It.IsAny<string>())).Returns("test");

            // Act & Assert
            await Assert.ThrowsAsync<CreateBoardColumnCommandSlugExistsException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async void GivenABoardColumnWhenBoardDoesNotExistThenThrowBoardNotFoundException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardColumnEntity>(), new List<BoardEntity>());

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new BoardColumn
                {
                    Slug = "test"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardColumnEntity>(It.IsAny<BoardColumn>()))
                .Returns(new BoardColumnEntity
                {
                    Name = "test",
                    Slug = "test"
                });

            mockSlugService.Setup(x => x.Slugify(It.IsAny<string>())).Returns("test");

            // Act & Assert
            await Assert.ThrowsAsync<BoardNotFoundException>(() => handler.HandleAsync(command));
        }
    }
}