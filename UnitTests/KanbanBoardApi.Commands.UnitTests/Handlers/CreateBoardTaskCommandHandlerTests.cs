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
    public class CreateBoardTaskCommandHandlerTests
    {
        private CreateBoardTaskCommandHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private Mock<ISlugService> mockSlugService;

        private void SetupCommandHandler(IList<BoardEntity> boards)
        {
            mockDataContext = new Mock<IDataContext>();

            var boardTaskDbSet = new FakeDbSet<BoardTaskEntity>();
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(boardTaskDbSet);

            var boardDbSet = new FakeDbSet<BoardEntity>();
            boards.ToList().ForEach(x => boardDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(boardDbSet);

            mockSlugService = new Mock<ISlugService>();

            mockMappingService = new Mock<IMappingService>();
            handler = new CreateBoardTaskCommandHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenABoardTaskWhenOkThenSaveChangedCalled()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity
                {
                    Slug = "board-name",
                    Columns = new List<BoardColumnEntity>
                    {
                        new BoardColumnEntity {Slug = "board-column-name"}
                    }
                }
            });

            var command = new CreateBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTask = new BoardTask
                {
                    BoardColumnSlug = "board-column-name",
                    Name = "test task"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardTaskEntity>(It.IsAny<BoardTask>()))
                .Returns(new BoardTaskEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenABoardTaskWhenOkThenBoardColumnAddedToBoardTask()
        {
            // Arrange
            var boardColumnEntity = new BoardColumnEntity {Slug = "board-column-name"};
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity
                {
                    Slug = "board-name",
                    Columns = new List<BoardColumnEntity>
                    {
                        boardColumnEntity
                    }
                }
            });

            var command = new CreateBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTask = new BoardTask
                {
                    BoardColumnSlug = "board-column-name",
                    Name = "test task"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardTaskEntity>(It.IsAny<BoardTask>()))
                .Returns(new BoardTaskEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            Assert.Equal(boardColumnEntity,
                mockDataContext.Object.Set<BoardTaskEntity>().Local.First().BoardColumnEntity);
        }

        [Fact]
        public async void GivenABoardTaskWhenOkThenMappingBackToDto()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity
                {
                    Slug = "board-name",
                    Columns = new List<BoardColumnEntity>
                    {
                        new BoardColumnEntity {Slug = "board-column-name"}
                    }
                }
            });

            var command = new CreateBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTask = new BoardTask
                {
                    BoardColumnSlug = "board-column-name",
                    Name = "test task"
                }
            };

            mockMappingService.Setup(x => x.Map<BoardTaskEntity>(It.IsAny<BoardTask>()))
                .Returns(new BoardTaskEntity());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<BoardTask>(It.IsAny<BoardTaskEntity>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardTaskWhenBoardDoesNotExistThenThrowBoardNotFoundException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>());

            var command = new CreateBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTask = new BoardTask
                {
                    Name = "test task"
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

        [Fact]
        public async void GivenABoardTaskWhenBoardColumnDoesNotExistThenThrowBoardColumnNotFoundException()
        {
            // Arrange
            SetupCommandHandler(new List<BoardEntity>
            {
                new BoardEntity { Slug = "board-name" }
            });

            var command = new CreateBoardTaskCommand
            {
                BoardSlug = "board-name",
                BoardTask = new BoardTask
                {
                    Name = "test task"
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
            await Assert.ThrowsAsync<BoardColumnNotFoundException>(() => handler.HandleAsync(command));
        }
    }
}