using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Commands.Exceptions;
using KanbanBoardApi.Commands.Handlers;
using KanbanBoardApi.Commands.Services;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;
using Moq;
using Xunit;

namespace KanbanBoardApi.Commands.UnitTests.Handlers
{
    public class CreateBoardColumnCommandHandlerTests
    {
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private Mock<ISlugService> mockSlugService;
        private CreateBoardColumnCommandHandler handler;

        private void SetupCommandHandler(IList<Domain.BoardColumn> boardColumns, IList<Domain.Board> boards)
        {
            mockDataContext = new Mock<IDataContext>();

            var boardColumnDbSet = new FakeDbSet<Domain.BoardColumn>();
            boardColumns.ToList().ForEach(x => boardColumnDbSet.Add(x));
            
            mockDataContext.Setup(x => x.Set<Domain.BoardColumn>()).Returns(boardColumnDbSet);

            var boardDbSet = new FakeDbSet<Domain.Board>();
            boards.ToList().ForEach(x => boardDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<Domain.Board>()).Returns(boardDbSet);

            mockSlugService = new Mock<ISlugService>();

            mockMappingService = new Mock<IMappingService>();
            handler = new CreateBoardColumnCommandHandler(mockDataContext.Object, mockMappingService.Object, mockSlugService.Object);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenSaveChangedCalled()
        {
            // Arrange
            SetupCommandHandler(new List<Domain.BoardColumn>(), new List<Domain.Board>
            {
                new Domain.Board { Slug = "test" }
            });
             
            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new Dto.BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<Domain.BoardColumn>(It.IsAny<Dto.BoardColumn>())).Returns(new Domain.BoardColumn());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenMappingBackToDto()
        {
            // Arrange
            SetupCommandHandler(new List<Domain.BoardColumn>(), new List<Domain.Board>
            {
                new Domain.Board { Slug = "test" }
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new Dto.BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<Domain.BoardColumn>(It.IsAny<Dto.BoardColumn>())).Returns(new Domain.BoardColumn());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockMappingService.Verify(x => x.Map<Dto.BoardColumn>(It.IsAny<Domain.BoardColumn>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenOkThenBoardColumnNameSlugified()
        {
            // Arrange
            SetupCommandHandler(new List<Domain.BoardColumn>(), new List<Domain.Board>
            {
                new Domain.Board { Slug = "test" }
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new Dto.BoardColumn()
            };

            mockMappingService.Setup(x => x.Map<Domain.BoardColumn>(It.IsAny<Dto.BoardColumn>())).Returns(new Domain.BoardColumn());

            // Act
            await handler.HandleAsync(command);

            // Assert
            mockSlugService.Verify(x => x.Slugify(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GivenABoardColumnWhenSlugAlreadyExistsThenThrowCreateBoardColumnCommandSlugExistsException()
        {
            // Arrange
            SetupCommandHandler(new List<Domain.BoardColumn>
            {
                new Domain.BoardColumn
                {
                    Id = 1,
                    Name = "test",
                    Slug = "test"
                }
            }, new List<Domain.Board>
            {
                new Domain.Board { Slug = "test" }
            });

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new Dto.BoardColumn
                {
                    Slug = "test"
                }
            };

            mockMappingService.Setup(x => x.Map<Domain.BoardColumn>(It.IsAny<Dto.BoardColumn>())).Returns(new Domain.BoardColumn
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
            SetupCommandHandler(new List<Domain.BoardColumn>(), new List<Domain.Board>());

            var command = new CreateBoardColumnCommand
            {
                BoardSlug = "test",
                BoardColumn = new Dto.BoardColumn
                {
                    Slug = "test"
                }
            };

            mockMappingService.Setup(x => x.Map<Domain.BoardColumn>(It.IsAny<Dto.BoardColumn>())).Returns(new Domain.BoardColumn
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