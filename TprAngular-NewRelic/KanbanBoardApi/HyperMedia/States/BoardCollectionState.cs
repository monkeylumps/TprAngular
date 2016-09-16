using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public class BoardCollectionState : IHyperMediaState
    {
        private readonly IBoardState boardState;
        private readonly ILinkFactory linkFactory;

        public BoardCollectionState(ILinkFactory linkFactory, IBoardState boardState)
        {
            this.linkFactory = linkFactory;
            this.boardState = boardState;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (BoardCollection);
        }

        public void Apply(object obj)
        {
            var boardCollection = obj as BoardCollection;

            if (boardCollection == null)
            {
                return;
            }

            boardCollection.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardSearch", new {})
                }
            };

            if (boardCollection.Items == null)
            {
                return;
            }

            foreach (var board in boardCollection.Items)
            {
                boardState.Apply(board);
            }
        }
    }
}