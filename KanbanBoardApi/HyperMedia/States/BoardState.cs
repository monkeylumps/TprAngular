using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public interface IBoardState : IHyperMediaState
    {
    }

    public class BoardState : IBoardState
    {
        private readonly ILinkFactory linkFactory;

        public BoardState(ILinkFactory linkFactory)
        {
            this.linkFactory = linkFactory;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (Board);
        }

        public void Apply(object obj)
        {
            var board = obj as Board;

            if (board == null)
            {
                return;
            }

            board.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardsGet", new
                    {
                        slug = board.Slug
                    })
                }
            };
        }
    }
}