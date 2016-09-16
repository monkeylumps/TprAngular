using System.Collections.Generic;
using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardTaskCollectionStateTests
    {
        private Mock<IBoardTaskState> mockBoardTaskState;
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardTaskCollectionState state;


        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            mockBoardTaskState = new Mock<IBoardTaskState>();
            state = new BoardTaskCollectionState(mockLinkFactory.Object, mockBoardTaskState.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardTaskCollectionThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var boardTaskCollection = new BoardTaskCollection();

            // Act
            var isAppliable = state.IsAppliable(boardTaskCollection);

            // Assert
            Assert.True(isAppliable);
        }

        [Fact]
        public void GiveAObjWhenIsNotABoardTaskCollectionThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var mockItem = new MockItem();

            // Act
            var isAppliable = state.IsAppliable(mockItem);

            // Assert
            Assert.False(isAppliable);
        }


        [Fact]
        public void GivenAObjWhenIsABoardTaskCollectionThenSelfLinkAdded()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardsSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardTaskCollection = new BoardTaskCollection();

            // Act
            state.Apply(boardTaskCollection);

            // Assert
            Assert.NotNull(boardTaskCollection.Links);
            Assert.NotNull(boardTaskCollection.Links.FirstOrDefault(x => x.Rel == Link.SELF));
        }

        [Fact]
        public void GivenAnObjectWhenIsABoardTaskCollectionThenApplyBoardStatesToAllBoards()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardsSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardTaskCollection = new BoardTaskCollection
            {
                Items = new List<BoardTask>
                {
                    new BoardTask()
                }
            };

            // Act
            state.Apply(boardTaskCollection);

            // Asset
            mockBoardTaskState.Verify(x => x.Apply(It.IsAny<BoardTask>()), Times.Once);
        }

        [Fact]
        public void GivenAObjWhenIsNotABoardTaskCollectionThenDoNothing()
        {
            // Arrange
            SetupState();
            var mockItem = new MockItem();

            // Act
            state.Apply(mockItem);
        }

        private class MockItem
        {
        }
    }
}