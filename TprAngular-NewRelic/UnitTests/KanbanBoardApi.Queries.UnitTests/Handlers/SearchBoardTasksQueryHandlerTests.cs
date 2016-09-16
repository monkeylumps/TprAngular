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
    public class SearchBoardTasksQueryHandlerTests
    {
        private SearchBoardTasksQueryHandler handler;
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;

        private void SetupQueryHandler(IList<BoardTaskEntity> data)
        {
            var fakeDbSet = new FakeDbSet<BoardTaskEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(x => x.Set<BoardTaskEntity>()).Returns(fakeDbSet);

            mockMappingService = new Mock<IMappingService>();

            handler = new SearchBoardTasksQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardsExistReturnBoardCollectionContainingBoards()
        {
            // Arrange
            SetupQueryHandler(new List<BoardTaskEntity>
            {
                new BoardTaskEntity
                {
                    BoardColumnEntity = new BoardColumnEntity
                    {
                        BoardEntity = new BoardEntity()
                    }
                }
            });

            mockMappingService.Setup(x => x.Map<BoardTask>(It.IsAny<BoardTaskEntity>())).Returns(new BoardTask());

            var query = new SearchBoardTasksQuery();

            // Act
            var boardTaskCollection = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardTaskCollection);
        }

        [Fact]
        public async void GiveQueryWhenBoardsDoesNotExistReturnEmptyBoardCollection()
        {
            // Arrange
            SetupQueryHandler(new List<BoardTaskEntity>());
            var query = new SearchBoardTasksQuery();

            // Act
            var boardTaskCollection = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardTaskCollection);
        }
    }
}