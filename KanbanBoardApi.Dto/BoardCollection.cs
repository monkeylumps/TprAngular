using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class BoardCollection : IHyperMediaItem
    {
        public IList<Board> Items { get; set; }

        public IList<Link> Links { get; set; }
    }
}