using KanbanBoardApi.Dto;

namespace KanbanBoardApi.Commands
{
    public class CreateBoardColumnCommand : ICommand
    {
        public string BoardSlug { get; set; }

        public BoardColumn BoardColumn { get; set; }
    }
}