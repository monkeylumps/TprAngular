using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public interface IHyperMediaItem
    {
        IList<Link> Links { get; set; }
    }
}