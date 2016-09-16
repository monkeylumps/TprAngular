using System.Collections.Generic;
using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardCollectionStateTests
    {
        private Mock<IBoardState> mockBoardState;
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardCollectionState state;


        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            mockBoardState = new Mock<IBoardState>();
            state = new BoardCollectionState(mockLinkFactory.Object, mockBoardState.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardCollectionThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var boardCollection = new BoardCollection();

            // Act
            var isAppliable = state.IsAppliable(boardCollection);

            // Assert
            Assert.True(isAppliable);
        }

        [Fact]
        public void GiveAObjWhenIsNotABoardCollectionThenReturnsTrue()
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
        public void GivenAObjWhenIsABoardCollectionThenSelfLinkAdded()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardCollection = new BoardCollection();

            // Act
            state.Apply(boardCollection);

            // Assert
            Assert.NotNull(boardCollection.Links);
            Assert.NotNull(boardCollection.Links.FirstOrDefault(x => x.Rel == Link.SELF));
        }

        [Fact]
        public void GivenAnObjectWhenIsABoardCollectionThenApplyBoardStatesToAllBoards()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardCollection = new BoardCollection
            {
                Items = new List<Board>
                {
                    new Board()
                }
            };

            // Act
            state.Apply(boardCollection);

            // Asset
            mockBoardState.Verify(x => x.Apply(It.IsAny<Board>()), Times.Once);
        }

        [Fact]
        public void GivenAObjWhenIsNotABoardCollectionThenDoNothing()
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