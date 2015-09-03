using KanbanBoardApi.Dto;

namespace KanbanBoardApi.Commands
{
    public class CreateBoardTaskCommand : ICommand
    {
        public string BoardSlug { get; set; }

        public BoardTask BoardTask { get; set; }
    }
}