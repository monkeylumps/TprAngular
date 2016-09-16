using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia
{
    public interface IHyperMediaFactory
    {
        void Apply(object obj);

        string GetLink(IHyperMediaItem obj, string linkType);
    }
}