using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Mapping;
using KanbanBoardApi.Queries.Handlers;
using Moq;
using Xunit;

namespace KanbanBoardApi.Queries.UnitTests.Handlers
{
    public class GetBoardColumnBySlugQueryHandlerTests
    {
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private GetBoardColumnBySlugQueryHandler handler;

        private void SetupQueryHandler(IList<Domain.Board> data)
        {
            var fakeDbSet = new FakeDbSet<Domain.Board>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<Domain.Board>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new GetBoardColumnBySlugQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsReturnsBoard()
        {
            // Arrange
            SetupQueryHandler(new List<Domain.Board>
            {
                new Domain.Board
                {
                    Slug = "board-name",
                    Columns = new List<Domain.BoardColumn>
                    {
                        new Domain.BoardColumn
                        {
                            Slug = "board-column-name"
                        }
                    }
                }
            });

            mockMappingService.Setup(x => x.Map<Dto.BoardColumn>(It.IsAny<Domain.BoardColumn>())).Returns(new Dto.BoardColumn());

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
            SetupQueryHandler(new List<Domain.Board>());
            var query = new GetBoardColumnBySlugQuery();

            // Act
            var boardColumn = await handler.HandleAsync(query);

            // Assert
            Assert.Null(boardColumn);
        }
    }
}