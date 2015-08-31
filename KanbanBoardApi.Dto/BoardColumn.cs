using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class BoardColumn : IHyperMediaItem
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Order { get; set; }

        public IList<Link> Links { get; set; }
    }
}