using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardStateTests
    {
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardState state;

        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            state = new BoardState(mockLinkFactory.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var board = new Board();

            // Act
            var isAppliable = state.IsAppliable(board);

            // Assert
            Assert.True(isAppliable);
        }

        [Fact]
        public void GiveAObjWhenIsNotABoardThenReturnsTrue()
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
        public void GivenAObjWhenIsABoardThenSelfLinkAdded()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardsGet", It.IsAny<object>())).Returns("http://fake-url/");
            var board = new Board();

            // Act
            state.Apply(board);

            // Assert
            Assert.NotNull(board.Links);
            Assert.NotNull(board.Links.FirstOrDefault(x => x.Rel == Link.SELF));
        }

        [Fact]
        public void GivenAObjWhenIsNotABoardThenDoNothing()
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