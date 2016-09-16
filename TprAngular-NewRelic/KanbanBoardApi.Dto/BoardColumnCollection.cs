using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class BoardColumnCollection
    {
        public IList<BoardColumn> Items { get; set; }

        public IList<Link> Links { get; set; }
    }
}