using System.Collections.Generic;
using System.Linq;
using FakeDbSet;
using KanbanBoardApi.Domain;
using KanbanBoardApi.Dto;
using KanbanBoardApi.EntityFramework;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.Mapping;
using KanbanBoardApi.Queries.Handlers;
using Moq;
using Xunit;

namespace KanbanBoardApi.Queries.UnitTests.Handlers
{
    public class SearchBoardColumnsQueryHandlerTests
    {
        private Mock<IDataContext> mockDataContext;
        private Mock<IMappingService> mockMappingService;
        private SearchBoardColumnsQueryHandler handler;

        private void SetupQueryHandler(IList<BoardEntity> data, IList<BoardColumnEntity> boardColumns)
        {
            mockMappingService = new Mock<IMappingService>();
            mockDataContext = new Mock<IDataContext>();

            var fakeDbSet = new FakeDbSet<BoardEntity>();
            data.ToList().ForEach(x => fakeDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardEntity>()).Returns(fakeDbSet);

            var fakeColumnDbSet = new FakeDbSet<BoardColumnEntity>();
            boardColumns.ToList().ForEach(x => fakeColumnDbSet.Add(x));
            mockDataContext.Setup(x => x.Set<BoardColumnEntity>()).Returns(fakeColumnDbSet);


            handler = new SearchBoardColumnsQueryHandler(mockDataContext.Object, mockMappingService.Object);
        }

        [Fact]
        public async void GivenQueryWhenBoardDoesNotExistThrowsBoardNotFoundException()
        {
            // Arrange
            SetupQueryHandler(new List<BoardEntity>(), new List<BoardColumnEntity>());
            var query = new SearchBoardColumnsQuery
            {
                BoardSlug = "board-name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BoardNotFoundException>(() => handler.HandleAsync(query));
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsThenBoardColumnCollectionReturned()
        {
            // Arrange
            var boardEntity = new BoardEntity {Slug = "board-name"};
            SetupQueryHandler(new List<BoardEntity>
            {
                boardEntity
            },
            new List<BoardColumnEntity>
            {
                new BoardColumnEntity
                {
                    BoardEntity = boardEntity
                }
            });

            var query = new SearchBoardColumnsQuery
            {
                BoardSlug = "board-name"
            };

            // Act
            var boardColumnCollection = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(boardColumnCollection);
        }

        [Fact]
        public async void GivenQueryWhenBoardExistsThenEachItemMapped()
        {
            // Arrange
            var boardEntity = new BoardEntity { Slug = "board-name" };
            SetupQueryHandler(new List<BoardEntity>
            {
                boardEntity
            },
            new List<BoardColumnEntity>
            {
                new BoardColumnEntity
                {
                    BoardEntity = boardEntity
                }
            });

            var query = new SearchBoardColumnsQuery
            {
                BoardSlug = "board-name"
            };

            // Act
            await handler.HandleAsync(query);

            // Assert
            mockMappingService.Verify(x => x.Map<BoardColumn>(It.IsAny<BoardColumnEntity>()), Times.Once);
        }
    }
}