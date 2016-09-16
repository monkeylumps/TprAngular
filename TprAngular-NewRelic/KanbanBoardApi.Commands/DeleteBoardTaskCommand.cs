namespace KanbanBoardApi.Commands
{
    public class DeleteBoardTaskCommand : ICommand
    {
        public string BoardSlug { get; set; }

        public int BoardTaskId { get; set; }
    }
}