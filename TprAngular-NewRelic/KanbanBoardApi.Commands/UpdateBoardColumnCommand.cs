using KanbanBoardApi.Dto;

namespace KanbanBoardApi.Commands
{
    public class UpdateBoardColumnCommand : ICommand
    {
        public string BoardSlug { get; set; }

        public BoardColumn BoardColumn { get; set; }
        public string BoardColumnSlug { get; set; }
    }
}