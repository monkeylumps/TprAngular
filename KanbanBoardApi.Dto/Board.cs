using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class Board : IHyperMediaItem
    {
        public string Slug { get; set; }

        public string Name { get; set; }


        public IList<Link> Links { get; set; }
    }
}