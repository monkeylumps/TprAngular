using System.Net.Http;
using SimpleInjector;

namespace KanbanBoardApi.HyperMedia
{
    public interface IRequestMessageProvider
    {
        HttpRequestMessage CurrentMessage { get; }

    }

    internal sealed class RequestMessageProvider : IRequestMessageProvider
    {
        private readonly Container container;

        public RequestMessageProvider(Container container)
        {
            this.container = container;
        }

        public HttpRequestMessage CurrentMessage
        {
            get { return this.container.GetCurrentHttpRequestMessage(); }
        }
    }
}