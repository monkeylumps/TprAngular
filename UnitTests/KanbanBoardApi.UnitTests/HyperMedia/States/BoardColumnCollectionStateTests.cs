using System.Collections.Generic;
using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardColumnCollectionStateTests
    {
        private Mock<IBoardColumnState> mockBoardColumnState;
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardColumnCollectionState state;


        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            mockBoardColumnState = new Mock<IBoardColumnState>();
            state = new BoardColumnCollectionState(mockLinkFactory.Object, mockBoardColumnState.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardColumnCollectionThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var boardColumnCollection = new BoardColumnCollection();

            // Act
            var isAppliable = state.IsAppliable(boardColumnCollection);

            // Assert
            Assert.True(isAppliable);
        }

        [Fact]
        public void GiveAObjWhenIsNotABoardColumnCollectionThenReturnsTrue()
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
        public void GivenAObjWhenIsABoardColumnCollectionThenSelfLinkAdded()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardColumnSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardColumnCollection = new BoardColumnCollection();

            // Act
            state.Apply(boardColumnCollection);

            // Assert
            Assert.NotNull(boardColumnCollection.Links);
            Assert.NotNull(boardColumnCollection.Links.FirstOrDefault(x => x.Rel == Link.SELF));
        }

        [Fact]
        public void GivenAnObjectWhenIsABoardColumnCollectionThenApplyBoardStatesToAllBoards()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardColumnSearch", It.IsAny<object>())).Returns("http://fake-url/");
            var boardColumnCollection = new BoardColumnCollection
            {
                Items = new List<BoardColumn>
                {
                    new BoardColumn()
                }
            };

            // Act
            state.Apply(boardColumnCollection);

            // Asset
            mockBoardColumnState.Verify(x => x.Apply(It.IsAny<BoardColumn>()), Times.Once);
        }

        [Fact]
        public void GivenAObjWhenIsNotABoardColumnCollectionThenDoNothing()
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