using System.Collections.Generic;
using KanbanBoardApi.Dto;

namespace KanbanBoardApi.HyperMedia.States
{
    public interface IBoardTaskState : IHyperMediaState
    {
    }

    public class BoardTaskState : IBoardTaskState
    {
        private readonly ILinkFactory linkFactory;

        public BoardTaskState(ILinkFactory linkFactory)
        {
            this.linkFactory = linkFactory;
        }

        public bool IsAppliable(object obj)
        {
            return obj.GetType() == typeof (BoardTask);
        }

        public void Apply(object obj)
        {
            var boardTask = obj as BoardTask;

            if (boardTask == null)
            {
                return;
            }

            boardTask.Links = new List<Link>
            {
                new Link
                {
                    Rel = Link.SELF,
                    Href = linkFactory.Build("BoardTaskGet", new
                    {
                        taskId = boardTask.Id,
                        boardSlug = linkFactory.GetRouteValue("boardSlug")
                    })
                }
            };
        }
    }
}