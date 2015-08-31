using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public interface IBoardColumnState : IHyperMediaState
    {
    }

    public class BoardColumnState : IBoardColumnState
    {
        private readonly ILinkFactory linkFactory;

        public BoardColumnState(ILinkFactory linkFactory)
        {
            this.linkFactory = linkFactory;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (BoardColumn);
        }

        public void Apply(object obj)
        {
            var boardColumn = obj as BoardColumn;

            if (boardColumn == null)
            {
                return;
            }

            boardColumn.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardColumnGet", new
                    {
                        boardSlug = linkFactory.GetRoutevalue("boardSlug"),
                        boardColumnSlug = boardColumn.Slug
                    })
                }
            };
        }
    }
}