using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardColumnStateTests
    {
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardColumnState state;
        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            state = new BoardColumnState(mockLinkFactory.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardColumnThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var boardColumn = new BoardColumn();

            // Act
            var isAppliable = state.IsAppliable(boardColumn);

            // Assert
            Assert.True(isAppliable);
        }

        [Fact]
        public void GiveAObjWhenIsNotABoardColumnThenReturnsTrue()
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
        public void GivenAObjWhenIsABoardColumnThenSelfLinkAdded()
        {
            // Arrange
            SetupState();
            mockLinkFactory.Setup(x => x.Build("BoardColumnsGet", It.IsAny<object>())).Returns("http://fake-url/");
            var boardColumn = new BoardColumn();

            // Act
            state.Apply(boardColumn);

            // Assert
            Assert.NotNull(boardColumn.Links);
            Assert.NotNull(boardColumn.Links.FirstOrDefault(x => x.Rel == Link.SELF));
        }

        [Fact]
        public void GivenAObjWhenIsNotABoardColumnThenDoNothing()
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