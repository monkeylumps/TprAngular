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
    public class GetBoardBySlugQueryHandlerTests
    {
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private GetBoardBySlugQueryHandler handler;

        private void SetupQueryHandler(IList<Domain.Board> data)
        {
            var fakeDbSet = new FakeDbSet<Domain.Board>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<Domain.Board>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new GetBoardBySlugQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsReturnsBoard()
        {
            // Arrange
            SetupQueryHandler(new List<Domain.Board>
            {
                new Domain.Board()
            });

            mockMappingService.Setup(x => x.Map<Dto.Board>(It.IsAny<Domain.Board>())).Returns(new Dto.Board());

            var query = new GetBoardBySlugQuery();

            // Act
            var board = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(board);
        }

        [Fact]
        public async void GiveQueryWhenBoardDoesNotExistReturnNull()
        {
            // Arrange
            SetupQueryHandler(new List<Domain.Board>());
            var query = new GetBoardBySlugQuery();

            // Act
            var board = await handler.HandleAsync(query);

            // Assert
            Assert.Null(board);
        }
    }
}