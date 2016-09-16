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
    public class GetBoardBySlugQueryHandlerTests
    {
        private GetBoardBySlugQueryHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupQueryHandler(IList<BoardEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new GetBoardBySlugQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsReturnsBoard()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>
            {
                new BoardEntity()
            });

            mockMappingService.Setup(x => x.Map<Board>(It.IsAny<BoardEntity>())).Returns(new Board());

            var query = new GetBoardBySlugQuery();

            // Act
            var board = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(board);
        }

        [Fact]
        public async void GivenQueryWhenBoardDoesNotExistReturnNull()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>());
            var query = new GetBoardBySlugQuery();

            // Act
            var board = await handler.HandleAsync(query);

            // Assert
            Assert.Null(board);
        }
    }
}