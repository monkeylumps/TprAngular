using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public interface IBoardTaskCollectionState : IHyperMediaState
    {
    }


    public class BoardTaskCollectionState : IBoardTaskCollectionState
    {
        private readonly ILinkFactory linkFactory;
        private readonly IBoardTaskState boardTaskState;

        public BoardTaskCollectionState(ILinkFactory linkFactory, IBoardTaskState boardTaskState)
        {
            this.linkFactory = linkFactory;
            this.boardTaskState = boardTaskState;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (BoardTaskCollection);
        }

        public void Apply(object obj)
        {
            var boardTaskCollection = obj as BoardTaskCollection;

            if (boardTaskCollection == null)
            {
                return;
            }

            boardTaskCollection.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardTasksSearch", new
                    {
                        boardSlug = linkFactory.GetRoutevalue("boardSlug")
                    })
                }
            };

            if (boardTaskCollection.Items == null)
            {
                return;
            }

            foreach (var board in boardTaskCollection.Items)
            {
                boardTaskState.Apply(board);
            }
        }
    }
}