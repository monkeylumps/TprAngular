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
    public class GetBoardTaskByIdQueryHandlerTests
    {
        private GetBoardTaskByIdQueryHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupQueryHandler(IList<BoardTaskEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardTaskEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new GetBoardTaskByIdQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsReturnsBoard()
        {
            // Arrange
            SetupQueryHandler(new List<BoardTaskEntity>
            {
                new BoardTaskEntity
                {
                    Id = 1
                }
            });

            mockMappingService.Setup(x => x.Map<BoardTask>(It.IsAny<BoardTaskEntity>())).Returns(new BoardTask());

            var query = new GetBoardTaskByIdQuery
            {
                TaskId = 1
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
            SetupQueryHandler(new List<BoardTaskEntity>());
            var query = new GetBoardTaskByIdQuery();

            // Act
            var boardColumn = await handler.HandleAsync(query);

            // Assert
            Assert.Null(boardColumn);
        }
    }
}