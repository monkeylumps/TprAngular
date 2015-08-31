using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;
using KanbanBoardApi.Queries.Handlers;
using Moq;
using Xunit;

namespace KanbanBoardApi.Queries.UnitTests.Handlers
{
    public class GetBoardColumnBySlugQueryHandlerTests
    {
        private GetBoardColumnBySlugQueryHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupQueryHandler(IList<BoardEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new GetBoardColumnBySlugQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsReturnsBoard()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>
            {
                new BoardEntity
                {
                    Slug = "board-name",
                    Columns = new List<BoardColumnEntity>
                    {
                        new BoardColumnEntity
                        {
                            Slug = "board-column-name"
                        }
                    }
                }
            });

            mockMappingService.Setup(x => x.Map<BoardColumn>(It.IsAny<BoardColumnEntity>())).Returns(new BoardColumn());

            var query = new GetBoardColumnBySlugQuery
            {
                BoardSlug = "board-name",
                BoardColumnSlug = "board-column-name"
            };

            // Act
            var boardColumn = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardColumn);
        }

        [Fact]
        public async void GivenQueryWhenBoardDoesNotExistReturnNull()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>());
            var query = new GetBoardColumnBySlugQuery();

            // Act
            var boardColumn = await handler.HandleAsync(query);

            // Assert
            Assert.Null(boardColumn);
        }
    }
}