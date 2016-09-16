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
    public class SearchBoardsQueryHandlerTests
    {
        private SearchBoardsQueryHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupQueryHandler(IList<BoardEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new SearchBoardsQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardsExistReturnBoardCollectionContainingBoards()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>
            {
                new BoardEntity()
            });

            mockMappingService.Setup(x => x.Map<Board>(It.IsAny<BoardEntity>())).Returns(new Board());

            var query = new SearchBoardsQuery();

            // Act
            var boardCollection = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardCollection);
        }

        [Fact]
        public async void GiveQueryWhenBoardsDoesNotExistReturnEmptyBoardCollection()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>());
            var query = new SearchBoardsQuery();

            // Act
            var boardCollection = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardCollection);
        }
    }
}