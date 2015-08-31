using KanbanBoardApi.Dto;

namespace KanbanBoardApi.Commands
{
    public class CreateBoardCommand : ICommand
    {
        public Board Board { get; set; }
    }
}