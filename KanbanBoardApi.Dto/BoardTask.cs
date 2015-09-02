using System.Collections.Generic;

namespace KanbanBoardApi.Dto
{
    public class BoardTask : IHyperMediaItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string BoardColumnSlug { get; set; }

        public IList<Link> Links { get; set; }
    }
}