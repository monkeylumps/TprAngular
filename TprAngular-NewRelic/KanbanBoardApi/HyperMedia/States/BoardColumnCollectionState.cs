using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public class BoardColumnCollectionState : IHyperMediaState
    {
        private readonly IBoardColumnState boardState;
        private readonly ILinkFactory linkFactory;

        public BoardColumnCollectionState(ILinkFactory linkFactory, IBoardColumnState boardState)
        {
            this.linkFactory = linkFactory;
            this.boardState = boardState;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (BoardColumnCollection);
        }

        public void Apply(object obj)
        {
            var boardColumnCollection = obj as BoardColumnCollection;

            if (boardColumnCollection == null)
            {
                return;
            }

            boardColumnCollection.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardColumnSearch", new {})
                }
            };

            if (boardColumnCollection.Items == null)
            {
                return;
            }

            foreach (var board in boardColumnCollection.Items)
            {
                boardState.Apply(board);
            }
        }
    }
}