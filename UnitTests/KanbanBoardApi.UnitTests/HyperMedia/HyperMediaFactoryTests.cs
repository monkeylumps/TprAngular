using System;
using System.Collections.Generic;
using KanbanBoardApi.Dto;
using KanbanBoardApi.HyperMedia;
using KanbanBoardApi.HyperMedia.Exceptions;
using KanbanBoardApi.HyperMedia.States;
using Moq;
using Xunit;

namespace KanbanBoardApi.UnitTests.HyperMedia
{
    public class HyperMediaFactoryTests
    {
        private HyperMediaFactory factory;
        private Func<IEnumerable<IHyperMediaState>> getHyperMediaStates = () => new List<IHyperMediaState>();

        private void SetupHyperMediaFactory()
        {
            factory = new HyperMediaFactory(getHyperMediaStates());
        }

        [Fact]
        public void GivenObjectAndLinkWhenLinkExistsThenReturnHref()
        {
            // Arrange
            SetupHyperMediaFactory();
            const string expectedLink = "http://link/";
            var testableHypermediaItem = new TestableHypermediaItem
            {
                Links = new List<Link>
                {
                    new Link
                    {
                        Rel = Link.SELF,
                        Href = expectedLink
                    }
                }
            };

            // Act
            var link = factory.GetLink(testableHypermediaItem, Link.SELF);

            // Assert
            Assert.Equal(expectedLink, link);
        }

        [Fact]
        public void GivenObjectAndLinkWhenLinkNotFoundThenThrowHyperMediaFactoryLinksNotFoundException()
        {
            // Arrange
            SetupHyperMediaFactory();
            var testableHypermediaItem = new TestableHypermediaItem
            {
                Links = new List<Link>()
            };

            // Act & Assert
            Assert.Throws<HyperMediaFactoryLinksNotFoundException>(
                () => factory.GetLink(testableHypermediaItem, Link.SELF));
        }


        [Fact]
        public void GivenObjectAndLinkWhenLinksNullThrowsHyperMediaFactoryLinksNullException()
        {
            // Arrange
            SetupHyperMediaFactory();
            var testableHypermediaItem = new TestableHypermediaItem();
            // Act & Assert
            Assert.Throws<HyperMediaFactoryLinksNullException>(() => factory.GetLink(testableHypermediaItem, Link.SELF));
        }


        [Fact]
        public void GivenObjectWhenIsAppliableStateThenStateIsApplied()
        {
            // Arrange
            var mockHyperMediaState = new Mock<IHyperMediaState>();
            mockHyperMediaState.Setup(x => x.IsAppliable(It.IsAny<object>())).Returns(true);
            var testableHypermediaItem = new TestableHypermediaItem();

            getHyperMediaStates = () => new List<IHyperMediaState>
            {
                mockHyperMediaState.Object
            };

            SetupHyperMediaFactory();
            // Act
            factory.Apply(testableHypermediaItem);

            // Assert
            mockHyperMediaState.Verify(x => x.Apply(It.IsAny<object>()), Times.Once);
            ;
        }

        private class TestableHypermediaItem : IHyperMediaItem
        {
            public IList<Link> Links { get; set; }
        }
    }
}