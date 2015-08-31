using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class BoardTaskCollection : IHyperMediaItem
    {
        public IList<BoardTask> Items { get; set; }

        public IList<Link> Links { get; set; }
    }
}