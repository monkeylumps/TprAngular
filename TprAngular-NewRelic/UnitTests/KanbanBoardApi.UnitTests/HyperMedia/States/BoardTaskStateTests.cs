using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia.States
{
    public class BoardTaskStateTests
    {
        private Mock<ILinkFactory> mockLinkFactory;
        private BoardTaskState state;

        private void SetupState()
        {
            mockLinkFactory = new Mock<ILinkFactory>();
            state = new BoardTaskState(mockLinkFactory.Object);
        }


        [Fact]
        public void GiveAObjWhenIsABoardThenReturnsTrue()
        {
            // Arrange
            SetupState();
            var boardTask = new BoardTask();

            // Act
            var isAppliable = state.IsAppliable(boardTask);

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
            mockLinkFactory.Setup(x => x.Build("BoardTaskGet", It.IsAny<object>())).Returns("http://fake-url/");
            var boardTask = new BoardTask();

            // Act
            state.Apply(boardTask);

            // Assert
            Assert.NotNull(boardTask.Links);
            Assert.NotNull(boardTask.Links.FirstOrDefault(x => x.Rel == Link.SELF));
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